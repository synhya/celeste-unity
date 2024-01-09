using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Actor : Entity
{
    private Tilemap tileMap;
    protected Room Room => Level.CurRoom;
    private Vector2 overrideSpeed;
    
    protected override void Start()
    {
        base.Start();
        Level.AllActors.Add(this);
        tileMap = Level.Map; 
    }

    protected virtual void Update()
    {
        
    }

    public void MoveH(float amount, Action<CollisionData> onCollide)
    {
        Remainder.x += amount;
        int move = Mathf.RoundToInt(Remainder.x);

        if (move != 0)
        {
            Remainder.x -= move;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideCheck(Vector2Int.right * sign))
                {
                    PositionWS += Vector2Int.right * sign;
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
        Remainder.y += amount;
        int move = Mathf.RoundToInt(Remainder.y);
            

        if (move != 0)
        {
            Remainder.y -= move;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideCheck(Vector2Int.up * sign))
                {
                    PositionWS += Vector2Int.up * sign;
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
    
    private Vector2Int GridPosMin => (Vector2Int)tileMap.WorldToCell((Vector3Int)HitBoxWS.min);
    private Vector2Int GridPosMax => (Vector2Int)tileMap.WorldToCell((Vector3Int)(HitBoxWS.max));
    
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
