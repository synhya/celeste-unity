
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
            
            // x part
            if (moveX != 0)
            {
                Remainder.x -= moveX;
                PositionWS += Vector2Int.right;
                if (moveX > 0)
                {
                    foreach (var actor in Level.AllActors)
                    {
                        if (OverlapCheck(actor))
                        {
                            // Push right
                            actor.MoveH(this.RightWS - actor.LeftWS, actor.Squish);
                        }
                        else if (riding.Contains(actor))
                        {
                            // Carry right
                            // there is no danger of squishing here
                            // nothing happens if actor hits wall!
                            actor.MoveH(moveX, null);
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
                            actor.MoveH(this.LeftWS - actor.RightWS, actor.Squish);
                        }
                        else if (riding.Contains(actor))
                        {
                            // Carry left
                            actor.MoveH(moveX, null);
                        }
                    }
                }
            }
            // y part
            if (moveY != 0)
            {
                Remainder.y -= moveY;
                PositionWS += Vector2Int.up;
                if (moveY > 0)
                {
                    foreach (var actor in Level.AllActors)
                    {
                        if (OverlapCheck(actor))
                        {
                            // Carry up may squish if hit ceiling
                            actor.MoveV(moveY, actor.Squish);
                        }
                    }
                }
                else
                {
                    foreach (var actor in Level.AllActors)
                    {
                        if (OverlapCheck(actor))
                        {
                            // Push down
                            actor.MoveV(this.DownWS - actor.UpWS, actor.Squish);
                        }
                        else if (riding.Contains(actor))
                        {
                            // Carry down
                            actor.MoveV(moveY, null);
                        }
                    }
                }
            }
        }
        
        //then turn collider on
        Collideable = true;
    }

    protected bool OverlapCheck(Actor actor)
    {
        return CollideCheck(actor);
    }
    private List<Actor> GetAllRidingActors()
    {
        // making list every update
        var ridingList = new List<Actor>();

        foreach (var actor in Level.AllActors)
        {
           if(actor.IsRiding(this))
               ridingList.Add(actor);
        }

        return ridingList;
    }
    protected bool HasActorRiding()
    {
        foreach (var actor in Level.AllActors)
        {
            if (actor.IsRiding(this))
                return true;
        }
        return false;
    }
}


