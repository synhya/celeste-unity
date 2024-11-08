﻿
// Entity 종류들. 
// Trigger, Killbox, Platform, WallBooster, JumpThru, SwapBlock, FlyFeather
//  They all have CollideCheck

using UnityEngine;

public abstract class Trigger : Entity
{
    [HideInInspector] public bool Triggered;

    protected override void Start()
    {
        base.Start();
        var room = GetComponentInParent<Room>();
        room.Triggers.Add(this);
    }

    public abstract void OnEnter(Entity other);
    public abstract void OnStay(Entity other);
    public abstract void OnLeave(Entity other);
}


