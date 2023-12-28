
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// every solids are tile based
/// </summary>
public class Solid : Entity
{
    protected override void Start()
    {
        base.Start();
        var room = GetComponentInParent<Room>();
        room.Solids.Add(this);
    }

    public void Move(float x, float y)
    {
        Remainder.x += x;
        Remainder.y += y;

        int moveX = Mathf.RoundToInt(Remainder.x);
        int moveY = Mathf.RoundToInt(Remainder.y);

        if (moveX != 0 || moveY != 0)
        {
            // loop through all actors in the level, add it to
            // a list if actor.IsRiding(this) is true.
            List<Actor> riding = GetAllRidingActors();
            
            // Make this Solid non-collidable for actors,
            // so that actors moved by it do not get stuck on it
            Collideable = false;

            if (moveX != 0)
            {
                Remainder.x -= moveX;
                PositionWS.x += moveX;
                if (moveX > 0)
                {
                    foreach (var actor in Level.AllActors)
                    {
                        if (OverlapCheck(actor))
                        {
                            // Push right
                            actor.MoveX(this.RightWS - actor.LeftWS, actor.Squish);
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
                            actor.MoveX(this.LeftWS - actor.RightWS, actor.Squish);
                        }
                        else if (riding.Contains(actor))
                        {
                            // Carry left
                            actor.MoveX(moveX, null);
                        }
                    }
                }
            }
    
            if (moveX != 0)
            {
                Remainder.x -= moveX;
                PositionWS.x += moveX;
                if (moveX > 0)
                {
                    foreach (var actor in Level.AllActors)
                    {
                        if (OverlapCheck(actor))
                        {
                            // Push right
                            actor.MoveX(this.RightWS - actor.LeftWS, actor.Squish);
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
                            actor.MoveX(this.LeftWS - actor.RightWS, actor.Squish);
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
        
        //then turn collider on
        Collideable = true;
    }

    private bool OverlapCheck(Actor actor)
    {
        // CollideCheck()
        throw new NotImplementedException();
    }
    private List<Actor> GetAllRidingActors()
    {
        return null;
    }
}


