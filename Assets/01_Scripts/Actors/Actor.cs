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

    public void MoveX(float amount, Action onCollide)
    {
        Remainder.x += amount;
        int move = Mathf.RoundToInt(Remainder.x);

        if (move != 0)
        {
            Remainder.x -= move;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideCheck( sign, 0))
                {
                    PositionWS.x += sign;
                    move -= sign;
                }
                else
                {
                    if (onCollide != null)
                        onCollide();
                    break;
                }
            }
        }
    }
    
    public void MoveY(float amount, Action onCollide)
    {
        Remainder.y += amount;
        int move = Mathf.RoundToInt(Remainder.y);
            

        if (move != 0)
        {
            Remainder.y -= move;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideCheck(0, sign))
                {
                    PositionWS.y += sign;
                    move -= sign;
                }
                else
                {
                    if (onCollide != null)
                        onCollide();
                    break;
                }
            }
        }
    }

    public virtual bool IsRiding(Solid[] solid) { return true; }
    public virtual void Squish() { }

    #region Collisions

    /// <summary>
    /// collision check with specific solid class
    /// </summary>
    protected bool CollideCheck<T>(int offsetX, int offsetY) where T : Solid
    {
        if (!Collideable) return false;
        
        HitboxBottomLeftOffset.x += offsetX;
        HitboxBottomLeftOffset.y += offsetY;

        bool ret = false;
        
        foreach (var solid in Room.Solids)
        {
            if (solid is T)
            {
                if (CollideEntity(solid))
                {
                    solid.OnTouchingActor();
                    ret = true;
                    break;
                }
            }
        }
        
        HitboxBottomLeftOffset.x -= offsetX;
        HitboxBottomLeftOffset.y -= offsetY;
        
        return ret;
    }

    // left to implement
    protected T CollideFirst<T>(int offsetX, int offsetY) where T : Solid
    {
        return null;
    }
    
    /// <summary>
    /// collision check with ground type tiles, every solid 
    /// </summary>
    protected bool CollideCheck(int offsetX, int offsetY)
    {
        var ret = OverlapTileFlagCheckOS(TileType.Ground, new Vector2(offsetX, offsetY), out var type, offsetX, offsetY);
        if (type.HasFlag(TileType.HalfGround) && 
            (Speed.y > 0 || OverlapTileFlagCheckOS(TileType.HalfGround, Vector2.zero)))
            ret = false;

        if (!ret)
        {
            HitboxBottomLeftOffset.x += offsetX;
            HitboxBottomLeftOffset.y += offsetY;
            ret = CollideSolidsCheck();
            HitboxBottomLeftOffset.x -= offsetX;
            HitboxBottomLeftOffset.y -= offsetY;
        }
        
        return ret;
    }
    
    private bool CollideSolidsCheck()
    {
        if (!Collideable) return false;
            
        // then check with each solid objects 
        foreach (var other in Room.Solids)
        {
            if (CollideEntity(other))
                return true;
        }
        
        return false;
    }

    #endregion
}
