
using System;
using System.Collections.Generic;
using UnityEngine;

public class Solid : Entity
{
    public bool Collidable = true;
    public Vector2 Position;

    public void Move(float x, float y)
    {
        xRemainder += x;
        yRemainder += y;

        int moveX = Mathf.RoundToInt(xRemainder);
        int moveY = Mathf.RoundToInt(yRemainder);

        if (moveX != 0 || moveY != 0)
        {
            // loop through all actors in the level, add it to
            // a list if actor.IsRiding(this) is true.
            List<Actor> riding = GetAllRidingActors();
            
            // Make this Solid non-collidable for actors,
            // so that actors moved by it do not get stuck on it
            Collidable = false;

            if (moveX != 0)
            {
                xRemainder -= moveX;
                Position.x += moveX;
                if (moveX > 0)
                {
                    foreach (var actor in Level.AllActors)
                    {
                        if (OverlapCheck(actor))
                        {
                            // Push right
                            actor.MoveX(this.Right - actor.Left, actor.Squish);
                        }
                        else if (riding.Contains(actor))
                        {
                            // Carry right
                            // there is no danger of squishing here
                            // nothing happens if actor hits wall!
                            actor.MoveX(moveX, null);
                        }
                    }
                }
                else
                {
                    foreach (var actor in Level.AllActors)
                    {
                        if (OverlapCheck(actor))
                        {
                            // Push left
                            actor.MoveX(this.Left - actor.Right, actor.Squish);
                        }
                        else if (riding.Contains(actor))
                        {
                            // Carry left
                            actor.MoveX(moveX, null);
                        }
                    }
                }
            }
        }
        
        // do same for the y-axis
    
        //then turn collider on
        Collidable = true;
    }

    private bool OverlapCheck(Actor actor)
    {
        throw new NotImplementedException();
    }
    private List<Actor> GetAllRidingActors()
    {
        return null;
    }
}


