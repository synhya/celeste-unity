using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// base class for solid and actor
/// </summary>
public abstract class Entity : MonoBehaviour
{
    protected Room Room;
    
    protected bool Collideable = true;
    protected bool IsSolid;
    
    // contains list of boxes player consume in OS(object space)
    public Collider2D HitBox;
    
    /// <summary>
    /// all entity move based on this value
    /// not transform.position!
    /// </summary>
    protected Vector3Int PositionWS;
    protected Vector2 Speed;
    protected Vector2 Remainder;

    public int RightWS => PositionWS.x + (int)HitBox.bounds.extents.x;
    public int LeftWS => PositionWS.x - (int)HitBox.bounds.extents.x;

    protected virtual void Start()
    {
        PositionWS = Vector3Int.RoundToInt(transform.position);
        
        if(!TryGetComponent(out HitBox)) 
            Debug.Log($"No Collider Found for entity {gameObject.name}");

        FindRoom();
    }

    protected virtual void FindRoom()
    {
        // Recurses upwards until it finds a valid component
        if(!(Room = GetComponentInParent<Room>())) 
            Debug.Log($"No Room Found for entity {gameObject.name}");
    }
    
    protected void UpdatePosition()
    {
        transform.position = PositionWS;
    }

    #region Collision

    protected bool CheckCollision(float offsetX, float offsetY)
    {
        // move hitbox by offset
        HitBox.offset += new Vector2(offsetX, offsetY);
        
        if (!IsSolid)
        {
            foreach (var other in Room.Solids)
            {
                if (EntityCollision(other))
                {
                    HitBox.offset -= new Vector2(offsetX, offsetY);
                    return true;
                }
            }
        }
        else
        {
            foreach (var other in Room.Actors)
            {
                if (EntityCollision(other)) 
                {
                    HitBox.offset -= new Vector2(offsetX, offsetY);
                    return true;
                }
            }
        }
        
        HitBox.offset -= new Vector2(offsetX, offsetY);
        return false;
    }

    private bool EntityCollision(Entity entity)
    {
        if (entity != null && entity != this && entity.Collideable
            && !entity.IsSolid && HitBox.IsTouching(entity.HitBox))
        {
            return true;
        }
        return false;
    }

    #endregion
    

    #region Utils

    /// <summary>
    /// Get "World Position" And Check tile type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected TileType? GetTileType(float x, float y)
    {
        foreach (var other in Room.Solids)
        {
            if (other != null && other != this && other.Collideable)
            {
                var targetPos = GetGridPosition(x, y);
                var tile = Room.Tilemap.GetTile(targetPos) as TypeTile;
                // tile.RefreshTile(targetPos, other.Tilemap);
                if (tile)
                    return tile.Type;
            }
        }
        
        return null;
    }

    protected TileType? GetTileType(Vector3Int pos)
    {
        return GetTileType(pos.x, pos.y);
    }
    
    
    public Vector3Int GetGridPosition(float x, float y, float z)
    {
        return Room.Tilemap.WorldToCell(new Vector3(x, y, z));
    }
    public Vector3Int GetGridPosition(float x, float y)
    {
        return GetGridPosition(x, y, 0);
    }
    public Vector3Int GetGridPosition(Vector3Int posWS)
    {
        return Room.Tilemap.WorldToCell(posWS);
    }

    #endregion
}
