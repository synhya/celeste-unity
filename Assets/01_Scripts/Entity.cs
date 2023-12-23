using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/// <summary>
/// base class for solid and actor
/// </summary>
public class Entity : MonoBehaviour
{
    protected Room Room;
    protected Level Level;

    protected bool Collideable = true;
    protected bool IsSolid;
    
    [Header("Entity Settings")]
    public Vector2Int HitboxBottomLeftOffset;
    public Vector2Int HitboxSize;

    [HideInInspector] public Vector2Int PositionWS;
    [HideInInspector] public Vector2Int PreviousPos;
    
    private RectInt varBox = new RectInt();
    public RectInt HitBoxWS
    {
        get {
            varBox.position = PositionWS + HitboxBottomLeftOffset;
            varBox.size = HitboxSize;
            return varBox;
        }
    }
    public Vector2Int GridPosMin => (Vector2Int)Level.Map.WorldToCell((Vector3Int)HitBoxWS.position);

    public Vector2Int GridPosMax
    {
        get {
            var h = HitBoxWS;
            return (Vector2Int)Level.Map.WorldToCell((Vector3Int)(h.position + h.size));
        }
    }
    public int RightWS => HitBoxWS.xMax;
    public int LeftWS => HitBoxWS.xMin;
    public Vector2 PosCenterWS => PositionWS + (Vector2)HitboxSize / 2f;
    public const int TileSize = 8;
    
    
    protected Vector2 Speed;
    protected Vector2 Remainder;


    protected virtual void Start()
    {
        FindRoom();
        Level = Room.Level;
        PositionWS = Vector2Int.RoundToInt(transform.position);
    }

    protected virtual void FindRoom()
    {
        // Recurses upwards until it finds a valid component
        if(!(Room = GetComponentInParent<Room>())) 
            Debug.Log($"No Room Found for entity {gameObject.name}");
    }
    
    protected void UpdatePosition()
    {
        if (PositionWS != PreviousPos)
        {
            transform.position = (Vector3Int)PositionWS;
            
        }
        
        PreviousPos = PositionWS;
    }

    

    #region Editor

    private void OnDrawGizmos()
    {
        // var min = GetGridPosMin();
        // var max = GetGridPosMax();
        //
        // Gizmos.DrawLine(new Vector3(min.x * TileSize, PositionWS.y, 0),
        //     new Vector3(max.x * TileSize, PositionWS.y, 0));
    }

    private void OnValidate()
    {
    }

    #endregion

    #region Collision

    protected bool CollideCheck(int offsetX, int offsetY)
    {
        HitboxBottomLeftOffset.x += offsetX;
        HitboxBottomLeftOffset.y += offsetY;
        var ret = CollideCheck();
        HitboxBottomLeftOffset.x -= offsetX;
        HitboxBottomLeftOffset.y -= offsetY;
        
        return ret;
    }

    private bool CollideCheck()
    {
        if (!Collideable) return false;
        
        var hitbox = HitBoxWS;
        
        if (!IsSolid)
        {
            // first check the room tilemap (only for actors)
            if (OverlapTileFlagCheckOS(TileType.Ground, Vector2.zero))
                return true;
            
            // then check with each solid objects 
            foreach (var other in Room.Solids)
            {
                if (EntityCollision(hitbox, other))
                    return true;
            }
        }
        else
        {
            foreach (var other in Room.Actors)
            {
                if (EntityCollision(hitbox, other)) 
                    return true;
            }
        }
        
        return false;
    }

    #region Tile Collisions

    /// <summary>
    /// overlap doesn't include "touching"
    /// </summary>
    /// <param name="dir">if 0, check for all grids in hitbox(inefficient)</param>
    /// <returns></returns>
    protected bool OverlapTileFlagCheckOS(TileType flag, Vector2 dir, out TileType tileType, int offsetX = 0, int offsetY = 0)
    {
        HitboxBottomLeftOffset.x -= offsetX;
        HitboxBottomLeftOffset.y -= offsetY;
        
        var bottomLeft = GridPosMin;
        var topRight = GridPosMax;
        
        if (dir.y < 0)
            topRight.y = bottomLeft.y;
        else if (dir.y > 0)
            bottomLeft.y = topRight.y;

        if (dir.x < 0)
            topRight.x = bottomLeft.x;
        else if (dir.x > 0)
            bottomLeft.x = topRight.x;

        var ret = CheckTileFlagInGrid(flag, bottomLeft, topRight, out tileType);
        
        HitboxBottomLeftOffset.x += offsetX;
        HitboxBottomLeftOffset.y += offsetY;

        return ret;
    }
    protected bool OverlapTileFlagCheckOS(TileType flag, Vector2 dir, int offsetX = 0, int offsetY = 0)
    {
        HitboxBottomLeftOffset.x += offsetX;
        HitboxBottomLeftOffset.y += offsetY;
        
        var bottomLeft = GridPosMin;
        var topRight = GridPosMax;
        
        if (dir.y < 0)
            topRight.y = bottomLeft.y;
        else if (dir.y > 0)
            bottomLeft.y = topRight.y;

        if (dir.x < 0)
            topRight.x = bottomLeft.x;
        else if (dir.x > 0)
            bottomLeft.x = topRight.x;

        var ret = CheckTileFlagInGrid(flag, bottomLeft, topRight);
        
        HitboxBottomLeftOffset.x -= offsetX;
        HitboxBottomLeftOffset.y -= offsetY;

        return ret;
    }
    private bool CheckTileFlagInGrid(TileType flag, Vector2Int min, Vector2Int max, out TileType tileType)
    {
        var tileRect = new RectInt();
        var pos = new Vector3Int();
        
        for(int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                pos.x = i;
                pos.y = j;
                var tile = Level.Map.GetTile(pos) as TypeTile;
                
                if (tile && tile.Type.HasFlag(flag))
                {
                    tileRect = tile.AABB;
                    var tilePosWS = Level.Map.GetCellCenterWorld(pos) 
                                    - Vector3.one * TileSize / 2;
                    tileRect.position += Vector2Int.RoundToInt(tilePosWS);

                    if (HitBoxWS.Overlaps(tileRect))
                    {
                        tileType = tile.Type;
                        return true;
                    }
                }
            }
        }
        tileType = TileType.None;
        return false;
    }
    private bool CheckTileFlagInGrid(TileType flag, Vector2Int min, Vector2Int max)
    {
        var tileRect = new RectInt();
        var pos = new Vector3Int();
        
        for(int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                pos.x = i;
                pos.y = j;
                var tile = Level.Map.GetTile(pos) as TypeTile;
                
                if (tile && tile.Type.HasFlag(flag))
                {
                    tileRect = tile.AABB;
                    var tilePosWS = Level.Map.GetCellCenterWorld(pos) 
                                    - Vector3.one * TileSize / 2;
                    tileRect.position += Vector2Int.RoundToInt(tilePosWS);

                    if (HitBoxWS.Overlaps(tileRect))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    #endregion

    #region Entity Collisions

    /// <summary>
    /// Check if there is entity from player position
    /// </summary>
    protected bool IsEntityTypeAt(int offsetX, int offsetY, Entity other)
    {
        HitboxBottomLeftOffset.x += offsetX;
        HitboxBottomLeftOffset.y += offsetY;
        var ret = EntityCollision(HitBoxWS, other, true);
        HitboxBottomLeftOffset.x -= offsetX;
        HitboxBottomLeftOffset.y -= offsetY;
        
        return ret;
    }
    
    private bool EntityCollision(RectInt hitbox, Entity other, bool ignoreCollideable = false)
    {
        if (other != null && other != this && (other.Collideable || ignoreCollideable)
            && other.IsSolid != IsSolid && hitbox.Overlaps(other.HitBoxWS))
        {
            return true;
        }
        return false;
    }

    #endregion

    #endregion
    
    #region GetTile

    /// <summary>
    /// Get Tile type from grid position
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    protected TileType? GetTileFromGridPos(Vector2Int gridPos)
    {
        var tile = Level.Map.GetTile((Vector3Int) gridPos) as TypeTile;
        // tile.RefreshTile(targetPos, other.Tilemap);
        if (tile)
            return tile.Type;
        
        return null;
    }
    
    protected TileType? GetTileFromWS(Vector2Int posWS)
    {
        var gridPos = (Vector2Int)Level.Map.WorldToCell((Vector3Int)posWS);
        
        return GetTileFromGridPos(gridPos);
    }
    
    protected TileType? GetTileFromOS(Vector2Int posOS)
    {
        var posWS = PositionWS + posOS;
        var gridPos = (Vector2Int)Level.Map.WorldToCell((Vector3Int)posWS);
        
        return GetTileFromGridPos(gridPos);
    }

    #endregion
}
