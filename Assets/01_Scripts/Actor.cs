using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actors don’t have any concept of their own velocity, acceleration, or gravity.
/// Every class that extends Actor takes care of that
/// </summary>
public class Actor : Entity
{
    
    public void MoveX(float amount, Action onCollide)
    {
        XRemainder += amount;
        int move = Mathf.RoundToInt(XRemainder);

        if (move != 0)
        {
            XRemainder -= move;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideAt(Position + new Vector2(sign, 0)))
                {
                    Position.x += sign;
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
        YRemainder += amount;
        int move = Mathf.RoundToInt(YRemainder);
            

        if (move != 0)
        {
            YRemainder -= move;
            int sign = (int)Mathf.Sign(move);

            while (move != 0)
            {
                if(!CollideAt(Position + new Vector2(sign, 0)))
                {
                    Position.y += sign;
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
