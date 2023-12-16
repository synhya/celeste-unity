
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// classic object
/// </summary>
public class Entity : MonoBehaviour
{
    protected Collider2D collisionChecker;
    protected bool solids = true;
    protected bool collideable = true;

    [HideInInspector] public float SpriteIdx;

    public Vector2 Position;
    
    /// <summary>
    /// called when room changes
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="???"></param>
    public virtual void Init()
    {
        
    }
    
    /// <summary>
    /// check if collide at AABB
    /// </summary>
    /// <param name="solids"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    protected virtual bool CollideAt(bool solids, Vector2 position)
    {
        // if caller is not solid -> check with solid only
        if (!solids)
        {
            foreach (var other in Game.Entities)
            {
                if (other != null && other != this && other.collideable &&
                    other.collisionChecker.OverlapPoint(position))
                    return true;
            }
        }
        else
        {
            // need to check collision for solids?
        }
        
        
        return false;
    }
}


