
using System;
using UnityEngine;

public class DashOrb : Trigger
{
    [SerializeField] private float RecreateTime = 4f;

    private float recreateTimer;
    private bool isCreated = true;

    protected override void Start()
    {
        base.Start();
        
        Collideable = false; // it is not "colliding" with player
    }
    public override void OnEnter(Entity other)
    {
        if (other is Player)
        {
            var p = other as Player;
            p.RefillDash();
        }
        
        // play disappear animation and timer 
        isCreated = false;
        recreateTimer = RecreateTime;
    }
    public override void OnStay(Entity other)
    {
    }
    public override void OnLeave(Entity other)
    {
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


