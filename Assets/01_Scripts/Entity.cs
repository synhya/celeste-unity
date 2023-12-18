using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// base class for solid and actor
/// </summary>
public abstract class Entity : MonoBehaviour
{
    protected RoomMananger Room => RoomMananger.Room;
    
    protected Collider2D CollisionChecker;
    protected bool Collideable = true;

    public Vector2 Position;
    protected float XRemainder;
    protected float YRemainder;

    public float Right => transform.position.x + CollisionChecker.bounds.extents.x;
    public float Left => transform.position.x - CollisionChecker.bounds.extents.x;
    
    protected bool CollideAt(Vector2 position)
    {
        foreach (var other in Room.Solids)
        {
            if (other != null && other != this &&
                other.Collideable && other.CollisionChecker.OverlapPoint(position))
                return true;
        }
        
        return false;
    }

    protected bool CollideAt(float x, float y)
    {
        return CollideAt(new Vector2(x, y));
    }
}
