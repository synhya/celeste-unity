using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Actors don’t have any concept of their own velocity, acceleration, or gravity.
/// Every class that extends Actor takes care of that
/// </summary>
public class Actor : Entity
{
    private Tilemap tileMap;
    protected Room Room => Level.CurrentRoom;
    private Vector2 overrideSpeed;
    
    protected override void Start()
    {
        base.Start();
        Level.AllActors.Add(this);
        tileMap = Level.Map; 
    }

    protected virtual void Update()
    {
        if (overrideSpeed.x != 0)
        {
            Speed.x = overrideSpeed.x;
            overrideSpeed.x = 0;
        }
        if (overrideSpeed.y != 0)
        {
            Speed.y = overrideSpeed.y;
            overrideSpeed.y = 0;
        }
    }
    
    protected void MoveTowardsH(int targetX, float speed)
    {
        var leftDist = targetX - PositionWS.x;
        var maxSpeed = leftDist / Time.deltaTime;
        overrideSpeed.x = Mathf.Sign(leftDist) * Mathf.Max(maxSpeed, speed);
    }

    protected void MoveTowardsV(int targetY, float speed)
    {
        var leftDist = targetY - PositionWS.y;
        var maxSpeed = leftDist / Time.deltaTime;
        overrideSpeed.y = Mathf.Sign(leftDist) * Mathf.Max(maxSpeed, speed);
    }

    public void MoveH(float amount, Action<CollisionData> onCollide)
    {
        Remainder += Vector2.right;
        int move = Mathf.RoundToInt(Remainder.x);

        if (move != 0)
        {
            Remainder -= Vector2.right;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideCheck(Vector2Int.right * sign))
                {
                    PositionWS += Vector2Int.right;
                    move -= sign;
                }
                else
                {
                    onCollide?.Invoke(null);
                    break;
                }
            }
        }
    }
    
    public void MoveV(float amount, Action<CollisionData> onCollide)
    {
        Remainder += Vector2.up;
        int move = Mathf.RoundToInt(Remainder.y);
            

        if (move != 0)
        {
            Remainder -= Vector2.up;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideCheck(Vector2Int.up * sign))
                {
                    PositionWS += Vector2Int.up;
                    move -= sign;
                }
                else
                {
                    onCollide?.Invoke(null);
                    break;
                }
            }
        }
    }

    private Solid lastSolidRiding;
    public Vector2 LiftSpeed
    {
        get => lastSolidRiding && IsRiding(lastSolidRiding) ? lastSolidRiding.Speed : Vector2.zero;
    }
    /// Typically, an Actor is riding a Solid if that Actor is immediately above the Solid.
    /// But some Actors might want to override this function to change how it behaves
    /// — for example, in TowerFall, players will also ride Solids
    /// that they are ledge grabbing on, and flying monsters never ride Solids.
    /// In Celeste, Madeline rides Solids when she stands on them or clings to the side of them.
    public virtual bool IsRiding(Solid solid)
    {
        if (!Collideable) return false;
        
        var was = PositionWS;
        PositionWS -= Vector2Int.up;
        var ret = CollideCheck(solid);
        PositionWS = was;

        lastSolidRiding = ret ? solid : null;

        return ret;
    }
    public virtual void Squish(CollisionData data) { }

    #region Collisions

    /// <summary>
    /// collision check with specific solid class
    /// </summary>
    protected bool CollideCheck<T>(Vector2Int offset) where T : Solid
    {
        if (!Collideable) return false;

        var was = PositionWS;
        PositionWS += offset;

        bool ret = false;
        
        foreach (var solid in Room.Solids)
        {
            if (solid is T)
            {
                if (CollideCheck(solid))
                {
                    ret = true;
                    break;
                }
            }
        }

        PositionWS = was;
        
        return ret;
    }


    // left to implement
    protected T CollideFirst<T>(Vector2Int offset) where T : Solid
    {
        return null;
    }
    
    /// <summary>
    /// collision check with ground type tiles + every solid 
    /// </summary>
    protected bool CollideCheck(Vector2Int offset)
    {
        if (!Collideable) return false;
        
        var ret = OverlapTileFlagCheckOS(TileType.Ground, offset);

        if (!ret)
        {
            var was = PositionWS;
            PositionWS += offset;
            
            foreach (var other in Room.Solids)
            {
                if (CollideCheck(other))
                {
                    ret = true;
                    break;
                }
            }

            PositionWS = was;
        }
        
        return ret;
    }

    #endregion
    
    #region Tile Collisions
    
    private Vector2Int GridPosMin => (Vector2Int)tileMap.WorldToCell((Vector3Int)HitBoxWS.position);

    private Vector2Int GridPosMax
    {
        get {
            var h = HitBoxWS;
            return (Vector2Int)tileMap.WorldToCell((Vector3Int)(h.position + h.size));
        }
    }
    
    /// <summary>
    /// overlap doesn't include "touching"
    /// </summary>
    /// <param name="type">the first tile overlapping</param>
    /// <returns></returns>
    protected bool OverlapTileFlagCheckOS(TileType flag, Vector2Int offset, out TileType type)
    {
        var was = PositionWS;
        PositionWS += offset;
        
        var min = GridPosMin;
        var max = GridPosMax;
        
        var tileRect = new RectInt();
        var pos = new Vector3Int();

        var ret = false;
        type = TileType.None;
        
        for(int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                pos.x = i;
                pos.y = j;
                var tile = tileMap.GetTile(pos) as TypeTile;
                
                if (tile && tile.Type.HasFlag(flag))
                {
                    tileRect = tile.AABB;
                    var tilePosWS = tileMap.GetCellCenterWorld(pos) 
                                    - Vector3.one * TileSize / 2;
                    tileRect.position += Vector2Int.RoundToInt(tilePosWS);

                    if (HitBoxWS.Overlaps(tileRect) && 
                        (flag != TileType.Ground || !tile.Type.HasFlag(TileType.HalfGround) ||
                         PositionWS.y - offset.y >= tilePosWS.y + TileSize))
                    {
                        // special case, if entity position.y + offset.y < tile center.y + Tilesize/2
                        // player's previous position is below tile so pass
                        
                        type = tile.Type;
                        ret = true;
                    }
                }
            }
        }
        PositionWS = was;

        return ret;
    }
    protected bool OverlapTileFlagCheckOS(TileType flag, Vector2Int offset)
    {
        return OverlapTileFlagCheckOS(flag, offset, out var t);
    }
    #endregion
}
