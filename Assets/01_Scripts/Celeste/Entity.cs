
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// classic object
/// </summary>
public class Entity : MonoBehaviour
{
    protected GameManager G;
    
    protected Collider2D collisionChecker;
    protected bool solids = true;
    protected bool collideable = true;
    protected Rect hitbox;

    
    public float SpriteIdx;
    public Vector2 Position;
    
    /// <summary>
    /// called when room changes
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="???"></param>
    public virtual void Init(GameManager G)
    {
        G = GameManager.G;
    }
    
    /// <summary>
    /// check if collide at AABB
    /// </summary>
    /// <param name="solids"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    protected bool CollideAt(Vector2 position)
    {
        // if caller is not solid -> check with solid only
        if (!solids)
        {
            foreach (var other in G.Entities)
            {
                if (other != null && other != this && other.solids &&
                    other.collideable && other.collisionChecker.OverlapPoint(position))
                    return true;
            }
        }
        else
        {
            // need to check collision for solids?
        }
        
        
        return false;
    }

    protected bool CollideAt(float x, float y)
    {
        return CollideAt(new Vector2(x, y));
    }
}


