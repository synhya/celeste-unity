using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// base class for solid and actor
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [Header("Offset Setting")] [Tooltip("Place Slightly above Entity's feet")]
    // In Order to Get Valid Grid Position
    [SerializeField] private float groundOffset = 0.001f;
    
    protected RoomMananger Room => RoomMananger.Room;
    
    protected Collider2D Hitbox;
    protected bool Collideable = true;

    public Vector2Int Position ; // => transform.position + groundOffset
    protected float XRemainder;
    protected float YRemainder;

    public float WorldRight => transform.position.x + Hitbox.bounds.extents.x;
    public float WorldLeft => transform.position.x - Hitbox.bounds.extents.x;
    
    /// <summary>
    /// Get "World Position" And Check tile type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected bool IsTile(TileType type ,int x, int y)
    {
        foreach (var other in Room.Solids)
        {
            if (other != null && other != this && other.Collideable)
            {
                var targetPos = new Vector3Int(x, y, 0);
                var tile = Room.Tilemap.GetTile(targetPos) as TypeTile;
                // tile.RefreshTile(targetPos, other.Tilemap);
                if (tile && tile.Type == type)
                    return true;
            }
        }
        
        return false;
    }

    #region Utils

    public Vector3Int GetGridPosition(float x, float y, float z)
    {
        return Room.Tilemap.WorldToCell(new Vector3(x, y, z));
    }
    public Vector3Int GetGridPosition(float x, float y)
    {
        return GetGridPosition(x, y, 0);
    }
    public Vector3Int GetGridPosition(Vector3 pos)
    {
        return Room.Tilemap.WorldToCell(pos);
    }

    #endregion
}
