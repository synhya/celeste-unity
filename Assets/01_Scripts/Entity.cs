using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/// <summary>
/// base class for solid and actor
/// </summary>
public abstract class Entity : MonoBehaviour
{
    protected Room Room;
    private Tilemap tileMap;

    protected bool Collideable = true;
    
    [Header("Entity Settings")]
    public Vector2Int HitboxBottomLeftOffset;
    public Vector2Int HitboxSize;

    [HideInInspector] public Vector2Int PositionWS;
    [HideInInspector] public Vector2Int PreviousPos;
    
    private RectInt varBox;
    public RectInt HitBoxWS
    {
        get {
            varBox.position = PositionWS + HitboxBottomLeftOffset;
            varBox.size = HitboxSize;
            return varBox;
        }
    }
    private Vector2Int GridPosMin => (Vector2Int)tileMap.WorldToCell((Vector3Int)HitBoxWS.position);

    private Vector2Int GridPosMax
    {
        get {
            var h = HitBoxWS;
            return (Vector2Int)tileMap.WorldToCell((Vector3Int)(h.position + h.size));
        }
    }
    
    public int RightWS => HitBoxWS.xMax;
    public int LeftWS => HitBoxWS.xMin;

    public const int TileSize = 8;
    
    
    protected Vector2 Speed;
    protected Vector2 Remainder;


    protected virtual void Start()
    {
        FindRoom();
        tileMap = Room.Level.Map; 
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
    
    protected bool CollideEntity(Entity other)
    {
        if (other != null && other != this && other.Collideable
            && HitBoxWS.Overlaps(other.HitBoxWS))
        {
            return true;
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
        HitboxBottomLeftOffset.x += offsetX;
        HitboxBottomLeftOffset.y += offsetY;
        
        var bottomLeft = GridPosMin;
        var topRight = GridPosMax;
        
        if (dir.y < 0)
            topRight.y = bottomLeft.y - offsetY;
        else if (dir.y > 0)
            bottomLeft.y = topRight.y - offsetY;

        if (dir.x < 0)
            topRight.x = bottomLeft.x - offsetX;
        else if (dir.x > 0)
            bottomLeft.x = topRight.x - offsetX;

        var ret = CheckTileFlagInGrid(flag, bottomLeft, topRight, out tileType);
        
        HitboxBottomLeftOffset.x -= offsetX;
        HitboxBottomLeftOffset.y -= offsetY;

        return ret;
    }
    protected bool OverlapTileFlagCheckOS(TileType flag, Vector2 dir, int offsetX = 0, int offsetY = 0)
    {
        HitboxBottomLeftOffset.x += offsetX;
        HitboxBottomLeftOffset.y += offsetY;
        
        var bottomLeft = GridPosMin;
        var topRight = GridPosMax;
        
        if (dir.y < 0)
            topRight.y = bottomLeft.y - offsetY;
        else if (dir.y > 0)
            bottomLeft.y = topRight.y - offsetY;

        if (dir.x < 0)
            topRight.x = bottomLeft.x - offsetX;
        else if (dir.x > 0)
            bottomLeft.x = topRight.x - offsetX;

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
                var tile = tileMap.GetTile(pos) as TypeTile;
                
                if (tile && tile.Type.HasFlag(flag))
                {
                    tileRect = tile.AABB;
                    var tilePosWS = tileMap.GetCellCenterWorld(pos) 
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
                var tile = tileMap.GetTile(pos) as TypeTile;
                
                if (tile && tile.Type.HasFlag(flag))
                {
                    tileRect = tile.AABB;
                    var tilePosWS = tileMap.GetCellCenterWorld(pos) 
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
}
