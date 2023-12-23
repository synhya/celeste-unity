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
        IsSolid = false;
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
    
}
