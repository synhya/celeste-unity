
using System;
using UnityEngine;

public class DashOrb : Solid
{
    [SerializeField] private float RecreateTime = 4f;

    private float recreateTimer;
    private bool isCreated = true;

    protected override void Start()
    {
        base.Start();
        
        Collideable = false; // it is not "colliding" with player
    }

    // refill if actor is player
    public override void OnTouchingActor(Actor actor)
    {
        if (actor is Player)
        {
            var p = actor as Player;
            p.RefillDash();

            

        }
        
        // play disappear animation and timer 
        isCreated = false;
        recreateTimer = RecreateTime;
    }

    private void Update()
    {
        if (recreateTimer > 0f)
        {
            
        } 
        else if (!isCreated)
        {
            // activate spriteRenderer and set Collidable to True
        }
    }
}


