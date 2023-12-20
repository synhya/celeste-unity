

using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    None,
    Grey,
    Spike,
    Grass,
    Ice,
    Fire,
}

/// <summary>
/// base tile for tiles in this project
/// </summary>
public class TypeTile : RuleTile
{
    [Header("Custom Settings")]
    
    public TileType Type = TileType.Grey;
    
    [Tooltip("0,0 ~ 8,8")]
    public RectInt AABB = new RectInt(0, 0, 8, 8);
    
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
        
        // find sprite and set AABB
    }
}


