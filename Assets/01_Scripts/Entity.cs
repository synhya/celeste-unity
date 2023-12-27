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

    #region Pos Shortcut

    public Vector2 CenterWS => PositionWS + Vector2.up * (HitboxSize.y * 0.5f);
    public Vector2 CenterRightWS => CenterWS + Vector2.right * (HitboxSize.x * 0.5f);
    public Vector2 CenterLeftWS => CenterWS - Vector2.right * (HitboxSize.x * 0.5f);
    public Vector2 BottonRightWS => PositionWS + Vector2.right * (HitboxSize.x * 0.5f);
    public Vector2 BottonLeftWS => PositionWS - Vector2.right * (HitboxSize.x * 0.5f);    

    #endregion

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

    protected virtual void FindRoom() => Room = GetComponentInParent<Room>();
    
    protected void UpdatePosition()
    {
        if (PositionWS != PreviousPos)
        {
            transform.position = (Vector3Int)PositionWS;
            
        }
        
        PreviousPos = PositionWS;
    }
    
    protected bool CollideCheck(Entity other)
    {
        if (other != this && other.Collideable
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
