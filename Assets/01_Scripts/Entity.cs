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
    public Room Room;
    
    protected bool Collideable = true;
    protected bool IsSolid;
    
    [Header("Entity Settings")]
    public Vector2Int HitboxBottomLeftOffset;
    public Vector2Int HitboxSize;

    [HideInInspector] public Vector2Int PositionWS;
    [HideInInspector] public Vector2Int PreviousPos;
    
    private RectInt hitBoxWS;
    private Vector2Int gridPosMin;
    private Vector2Int gridPosMax;
    
    public RectInt GetHitBoxWS()
    {
        hitBoxWS.position = PositionWS + HitboxBottomLeftOffset;
        return hitBoxWS;
    }

    public Vector2Int GetGridPosMin()
    {
        var hitbox = GetHitBoxWS();
        
        gridPosMin = (Vector2Int)Room.Tilemap.WorldToCell((Vector3Int)hitbox.position)
                     + Vector2Int.left;
        
        return gridPosMin;
    }
    public Vector2Int GetGridPosMax()
    {
        var hitbox = GetHitBoxWS();
        
        gridPosMax = (Vector2Int)Room.Tilemap.WorldToCell((Vector3Int)(hitbox.position + hitbox.size))
                     + Vector2Int.right;
        
        return gridPosMin;
    }

    [HideInInspector] public RectInt TileRect = new RectInt()
    {
        width = TileSize,
        height = TileSize,
    };
    
    protected Vector2 Speed;
    public Vector2 Remainder;


    public int RightWS => GetHitBoxWS().xMax;
    public int LeftWS => GetHitBoxWS().xMax;

    private const int TileSize = 8;

    protected virtual void Start()
    {
        FindRoom();
        
        PositionWS = Vector2Int.RoundToInt(transform.position);
        hitBoxWS = new RectInt
        {
            x = PositionWS.x + HitboxBottomLeftOffset.x,
            y = PositionWS.y + HitboxBottomLeftOffset.y,
            width = HitboxSize.x,
            height = HitboxSize.y
        };
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

    

    #region Gizmo

    private void OnDrawGizmos()
    {
        // var min = GetGridPosMin();
        // var max = GetGridPosMax();
        //
        // Gizmos.DrawLine(new Vector3(min.x * TileSize, PositionWS.y, 0),
        //     new Vector3(max.x * TileSize, PositionWS.y, 0));
    }

    #endregion

    #region Collision

    protected bool CheckCollision(int offsetX, int offsetY)
    {
        HitboxBottomLeftOffset.x += offsetX;
        HitboxBottomLeftOffset.x += offsetY;
        var ret = CheckCollision();
        HitboxBottomLeftOffset.x -= offsetX;
        HitboxBottomLeftOffset.x -= offsetY;
        
        return ret;
    }

    private bool CheckCollision()
    {
        var hitbox = GetHitBoxWS();
        
        if (!IsSolid)
        {
            // first check the room tilemap (only for actors)
            if (CheckRoomTileCollision(hitbox))
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
    private bool CheckRoomTileCollision(RectInt hitbox)
    {
        Vector3Int pos = new Vector3Int();
        
        var min = GetGridPosMin();
        var max = GetGridPosMax();
        
        for(int i = min.x; i <= gridPosMax.x; i++)
        {
            for (int j = max.y; j <= gridPosMax.y; j++)
            {
                pos.x = i;
                pos.y = j;
                if (Room.Tilemap.HasTile(pos))
                {
                    var tileCenter = Room.Tilemap.GetCellCenterWorld(pos);
                    TileRect.x = (int)tileCenter.x - TileSize / 2;
                    TileRect.y = (int)tileCenter.y - TileSize / 2;

                    if (hitbox.Overlaps(TileRect))
                    {
                        return true;
                    }
                        
                }
            }
        }
        return false;
    }

    private bool EntityCollision(RectInt hitbox, Entity other)
    {
        if (other != null && other != this && other.Collideable
            && other.IsSolid != IsSolid && hitbox.Overlaps(other.GetHitBoxWS()))
        {
            return true;
        }
        return false;
    }

    #endregion
    

    #region Utils

    #region GetTile

    /// <summary>
    /// Get Tile type from grid position
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    protected TileType? GetTileFromGridPos(Vector2Int gridPos)
    {
        var tile = Room.Tilemap.GetTile((Vector3Int) gridPos) as TypeTile;
        // tile.RefreshTile(targetPos, other.Tilemap);
        if (tile)
            return tile.Type;
        
        return null;
    }
    
    protected TileType? GetTileFromWS(Vector2Int posWS)
    {
        var gridPos = (Vector2Int)Room.Tilemap.WorldToCell((Vector3Int)posWS);
        
        return GetTileFromGridPos(gridPos);
    }

    #endregion
    
    #endregion
}
