using System;
using System.Collections;
using UnityEngine;


public class Player : Actor
{
    public const int StNormal = 0;
    public const int StClimb = 1;
    public const int StDash = 2;

    #region vars

    public Animator Animator;
    public StateMachine StateMachine;

    public Vector2 PreviousPosition;
    public Vector2 Speed;
    
    private float idleTimer;
    
    #endregion

    public override void Init(GameManager G)
    {
        base.Init(G);
        
        StateMachine = StateMachine.AttachStateMachine(this.gameObject, 23);
        
        StateMachine.SetCallbacks(StNormal, NormalUpdate, null, NormalBegin, NormalEnd);
        StateMachine.SetCallbacks(StDash, null, DashCoroutine(), null, null);
        
    }

    #region Updating

    // TODO: change to StateMachine behavior
    public void Update()
    {
        if (G.PausePlayer) return;
        var input = Input.GetAxisRaw("Horizontal");
        
        // spikes collide
        if (G.SpikesAt(collisionChecker, Speed))
            G.KillPlayer(this);
        
        // bottom death
        if (Position.y < 0)
            G.KillPlayer(this);

        // basically downward is positive in XNA
        var onGround = CollideAt(0, -1);
    }

    #endregion

    #region Normal State

    private void NormalBegin()
    {
        
    }

    private void NormalEnd()
    {
        
    }

    private int NormalUpdate()
    {
        // start from here
        return StNormal;
    }

    #endregion



    #region Dash State

    private IEnumerator DashCoroutine()
    {
        yield return null;
    }

    #endregion
}
