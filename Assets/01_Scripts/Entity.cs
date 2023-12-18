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
    
    protected bool Collideable = true;

    // to get,set world position
    public Vector3Int PositionWS;
    
    // contains list of boxes player consume in OS(object space)
    public Vector2[] hitBox;

    protected Vector2 Speed;
    protected Vector2 Remainder;

    public float WorldRight => transform.position.x ;
    public float WorldLeft => transform.position.x;

    protected virtual void Start()
    {
        PositionWS = Vector3Int.RoundToInt(transform.position);
    }

    protected virtual void LateUpdate()
    {
         transform.position = PositionWS;
    }

    /// <summary>
    /// Get "World Position" And Check tile type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected bool IsTile(TileType type ,float x, float y)
    {
        foreach (var other in Room.Solids)
        {
            if (other != null && other != this && other.Collideable)
            {
                var targetPos = GetGridPosition(x, y);
                var tile = Room.Tilemap.GetTile(targetPos) as TypeTile;
                // tile.RefreshTile(targetPos, other.Tilemap);
                if (tile && tile.Type == type)
                    return true;
            }
        }
        
        return false;
    }

    protected bool IsTile(TileType type, Vector3 pos)
    {
        return IsTile(type, pos.x, pos.y);
    }
    

    #region Utils

    public void SetX(int value)
    {
        PositionWS = new Vector3Int(PositionWS.x, value, PositionWS.z);
    }

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
