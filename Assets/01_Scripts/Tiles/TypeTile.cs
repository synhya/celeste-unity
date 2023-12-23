

using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[Flags]
public enum TileType
{
    None = 0,

    // ground (collide-able)  
    Snow = 1 << 0,
    Sand = 1 << 1,
    Ice= 1 << 2, 
    Metal = 1 << 3, 
    Wood = 1 << 4, 
    
    // obstacles
    Spike = 1 << 15,
    Water = 1 << 16,
    Fire = 1 << 17,
    
    // attribute
    Ground = 1 << 21,  
    HalfGround = 1 << 22 | Ground,
    Obstacle = 1 << 23,
    Effect = 1 << 24,  // 플레이어의 움직임에 반응하는 등의 용도
}

/// <summary>
/// base tile for tiles in this project
/// </summary>
public class TypeTile : RuleTile
{
    [Header("Custom Settings")]
    
    public TileType Type = TileType.Snow;
    [FormerlySerializedAs("RuleEffected")]
    public bool EffectedByOtherRuleTile = true;
    
    [Tooltip("0,0 ~ 8,8")]
    public RectInt AABB = new RectInt(0, 0, 8, 8);
    
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
        
        // find sprite and set AABB
    }
    
    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other is RuleOverrideTile)
            other = (other as RuleOverrideTile).m_InstanceTile;

        switch (neighbor)
        {
            case TilingRule.Neighbor.This:
            {
                return other is TypeTile
                       && (other as TypeTile).Type.HasFlag(TileType.Ground) 
                       && Type.HasFlag(TileType.Ground) && 
                       ((other as TypeTile).EffectedByOtherRuleTile && EffectedByOtherRuleTile ||
                       (other as TypeTile).Type == this.Type);
            }
            case TilingRule.Neighbor.NotThis:
            {
                return !(other is TypeTile
                         && (other as TypeTile).Type.HasFlag(TileType.Ground)
                         && Type.HasFlag(TileType.Ground) && 
                         ((other as TypeTile).EffectedByOtherRuleTile && EffectedByOtherRuleTile ||
                          (other as TypeTile).Type == this.Type));
            }
        }

        return base.RuleMatch(neighbor, other);
    }
}


