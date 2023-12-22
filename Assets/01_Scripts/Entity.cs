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
    [HideInInspector] public Room Room;

    protected bool Collideable = true;
    protected bool IsSolid;
    
    [Header("Entity Settings")]
    public Vector2Int HitboxBottomLeftOffset;
    public Vector2Int HitboxSize;

    [HideInInspector] public Vector2Int PositionWS;
    [HideInInspector] public Vector2Int PreviousPos;
    
    public RectInt HitBoxWS => new RectInt(PositionWS + HitboxBottomLeftOffset, HitboxSize);
    public Vector2Int GridPosMin => (Vector2Int)Room.StaticTilemap.WorldToCell((Vector3Int)HitBoxWS.position);

    public Vector2Int GridPosMax
    {
        get {
            var h = HitBoxWS;
            return (Vector2Int)Room.StaticTilemap.WorldToCell((Vector3Int)(h.position + h.size));
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

    protected bool CheckCollision(int offsetX, int offsetY)
    {
        HitboxBottomLeftOffset.x += offsetX;
        HitboxBottomLeftOffset.y += offsetY;
        var ret = CheckCollision();
        HitboxBottomLeftOffset.x -= offsetX;
        HitboxBottomLeftOffset.y -= offsetY;
        
        return ret;
    }

    private bool CheckCollision()
    {
        var hitbox = HitBoxWS;
        
        if (!IsSolid)
        {
            // first check the room tilemap (only for actors)
            if (IsTouchingStaticTileType(TileType.Grey))
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
    
    protected bool IsTouchingStaticTileType(TileType targetType)
    {
        var tileRect = new RectInt();
        var pos = new Vector3Int();
        
        var min = GridPosMin;
        var max = GridPosMax;
        
        for(int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                pos.x = i;
                pos.y = j;
                var tile = Room.StaticTilemap.GetTile(pos) as TypeTile;
                
                if (tile && tile.Type == targetType)
                {
                    tileRect = tile.AABB;
                    var tilePosWS = Room.StaticTilemap.GetCellCenterWorld(pos) 
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
    
    protected bool IsStandingAtStaticTileType(TileType targetType)
    {
        var tileRect = new RectInt();
        var pos = new Vector3Int();
        
        var min = GridPosMin;
        var max = GridPosMax;
        
        for(int i = min.x; i <= max.x; i++)
        {
            int j = min.y; // for all bottom blocks
            pos.x = i;
            pos.y = j;
            var tile = Room.StaticTilemap.GetTile(pos) as TypeTile;
            
            if (tile && tile.Type == targetType)
            {
                tileRect = tile.AABB;
                var tilePosWS = Room.StaticTilemap.GetCellCenterWorld(pos) 
                                - Vector3.one * TileSize / 2;
                tileRect.position += Vector2Int.RoundToInt(tilePosWS);

                if (HitBoxWS.Overlaps(tileRect))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Check if there is entity from player position
    /// </summary>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    /// <param name="other"></param>
    /// <returns></returns>
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
    
    #region GetTile

    /// <summary>
    /// Get Tile type from grid position
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    protected TileType? GetTileFromGridPos(Vector2Int gridPos)
    {
        var tile = Room.StaticTilemap.GetTile((Vector3Int) gridPos) as TypeTile;
        // tile.RefreshTile(targetPos, other.Tilemap);
        if (tile)
            return tile.Type;
        
        return null;
    }
    
    protected TileType? GetTileFromWS(Vector2Int posWS)
    {
        var gridPos = (Vector2Int)Room.StaticTilemap.WorldToCell((Vector3Int)posWS);
        
        return GetTileFromGridPos(gridPos);
    }
    
    protected TileType? GetTileFromOS(Vector2Int posOS)
    {
        var posWS = PositionWS + posOS;
        var gridPos = (Vector2Int)Room.StaticTilemap.WorldToCell((Vector3Int)posWS);
        
        return GetTileFromGridPos(gridPos);
    }

    #endregion
}
