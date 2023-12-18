using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actors don’t have any concept of their own velocity, acceleration, or gravity.
/// Every class that extends Actor takes care of that
/// </summary>
public class Actor : Entity
{
    
    public override void Init(GameManager G)
    {
        base.Init(G);

        solids = false;
        Level.AllActors.Add(this);
    }

    public void MoveX(float amount, Action onCollide)
    {
        xRemainder += amount;
        int move = Mathf.RoundToInt(xRemainder);

        if (move != 0)
        {
            xRemainder -= move;
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
        yRemainder += amount;
        int move = Mathf.RoundToInt(yRemainder);
            

        if (move != 0)
        {
            yRemainder -= move;
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

