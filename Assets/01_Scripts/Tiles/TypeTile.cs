

using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Flags]
public enum TileType
{
    None = 0,
    
    // ground (collide-able)  
    SnowG = 1 | Ground | Effect,
    GrassG = 2 | Ground,
    IceG = 3 | Ground,
    MetalG = 4 | Ground,
    
    // deco tiles with effect
    SnowD = 1 | Effect,
    
    // obstacles
    SpikeO = 1 | Obstacle,
    WaterO = 2 | Obstacle,
    FireO = 3 | Obstacle,
    
    // attribute
    [InspectorName(null)]
    Ground = 1 << 5,  // 32 (MAX)
    [InspectorName(null)]
    Obstacle = 1 << 6,
    [InspectorName(null)]
    Effect = 1 << 7,  // 플레이어의 움직임에 반응하는 등의 용도
}

/// <summary>
/// base tile for tiles in this project
/// </summary>
public class TypeTile : RuleTile
{
    [Header("Custom Settings")]
    
    public TileType Type = TileType.SnowG;
    
    [Tooltip("0,0 ~ 8,8")]
    public RectInt AABB = new RectInt(0, 0, 8, 8);
    
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
        
        // find sprite and set AABB
    }
}


