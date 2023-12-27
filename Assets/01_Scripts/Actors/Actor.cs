using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actors don’t have any concept of their own velocity, acceleration, or gravity.
/// Every class that extends Actor takes care of that
/// </summary>
public class Actor : Entity
{
    protected override void Start()
    {
        base.Start();
        Room.OnActorEnter(this);
    }

    public void MoveX(float amount, Action<CollisionData> onCollide)
    {
        Remainder.x += amount;
        int move = Mathf.RoundToInt(Remainder.x);

        if (move != 0)
        {
            Remainder.x -= move;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideCheck(Vector2Int.right * sign))
                {
                    PositionWS.x += sign;
                    move -= sign;
                }
                else
                {
                    // if (onCollide != null)
                    //     onCollide();
                    break;
                }
            }
        }
    }
    
    public void MoveY(float amount, Action<CollisionData> onCollide)
    {
        Remainder.y += amount;
        int move = Mathf.RoundToInt(Remainder.y);
            

        if (move != 0)
        {
            Remainder.y -= move;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideCheck(Vector2Int.up * sign))
                {
                    PositionWS.y += sign;
                    move -= sign;
                }
                else
                {
                    // if (onCollide != null)
                    //     onCollide();
                    break;
                }
            }
        }
    }

    public virtual bool IsRiding(Solid[] solid) { return true; }
    public virtual void Squish(CollisionData data) { }

    #region Collisions

    /// <summary>
    /// collision check with specific solid class
    /// </summary>
    protected bool CollideCheck<T>(Vector2Int offset) where T : Solid
    {
        if (!Collideable) return false;

        var was = PositionWS;
        PositionWS += offset;

        bool ret = false;
        
        foreach (var solid in Room.Solids)
        {
            if (solid is T)
            {
                if (CollideCheck(solid))
                {
                    solid.OnTouchingActor(this);
                    ret = true;
                    break;
                }
            }
        }

        PositionWS = was;
        
        return ret;
    }


    // left to implement
    protected T CollideFirst<T>(Vector2Int offset) where T : Solid
    {
        return null;
    }
    
    /// <summary>
    /// collision check with ground type tiles + every solid 
    /// </summary>
    protected bool CollideCheck(Vector2Int offset)
    {
        if (!Collideable) return false;
        
        var ret = OverlapTileFlagCheckOS(TileType.Ground, offset);

        if (!ret)
        {
            var was = PositionWS;
            PositionWS += offset;
            
            foreach (var other in Room.Solids)
            {
                if (CollideCheck(other))
                {
                    ret = true;
                    break;
                }
            }

            PositionWS = was;
        }
        
        return ret;
    }

    #endregion
}
