﻿
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
        Room.Solids.Add(this);
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
                    foreach (var actor in Room.Actors)
                    {
                        if (OverlapCheck(actor))
                        {
                            // Push right
                            actor.MoveX(this.WorldRight - actor.WorldLeft, actor.Squish);
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
                    foreach (var actor in Room.Actors)
                    {
                        if (OverlapCheck(actor))
                        {
                            // Push left
                            actor.MoveX(this.WorldLeft - actor.WorldRight, actor.Squish);
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
        Collideable = true;
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


