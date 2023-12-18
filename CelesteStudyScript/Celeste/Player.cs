using System;
using System.Collections;
using UnityEngine;

using static UnityEngine.Mathf;
using Random = UnityEngine.Random;

public class Player : Actor
{
    public const int StNormal = 0;
    public const int StClimb = 1;
    public const int StDash = 2;

    #region vars

    [HideInInspector] public SpriteRenderer SpriteRenderer;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public StateMachine StateMachine;

    private Vector2 previousPosition;
    [SerializeField] private Vector2 speed;
    
    private float idleTimer;
    private float dashTime;

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
        if (G.SpikesAt(collisionChecker, speed))
            G.KillPlayer(this);
        
        // bottom death
        if (Position.y < 0)
            G.KillPlayer(this);

        // basically downward is positive in XNA
        var onGround = CollideAt(0, -1);

        if (dashTime > 0)
        {
            // smoke emitter.
        }
        else
        {
            // move
            var maxrun = 1;
            var accel = 0.6f;
            var deccel = 0.15f;

            if (!onGround)
                accel = 0.4f;

            // speed limit
            var spdXAbs = Abs(speed.x);
            var spdXSign = Sign(speed.x);
            if (spdXAbs > maxrun)
                speed.x = spdXSign * Max(spdXAbs - deccel, maxrun);
            else
                speed.x = spdXSign * Max(spdXAbs + accel, maxrun);

            // facing
            if (speed.x != 0)
                SpriteRenderer.flipX = (int)spdXSign == 1;
            
            // gravity
            var maxfall = 2f;
            var gravity = 0.21f;

            if (Abs(speed.y) <= 0.15f)
                gravity *= 0.5f;
            
            // wall slide
            if (input != 0 && CollideAt(input, 0))
            {
                maxfall = 0.4f;
                // if(Random.Range(0, 10) < 2) 
                    // emit smoke
            }

            if (!onGround)
                speed.y = Sign(speed.y) * Max(Abs(speed.y) + gravity, maxfall);
        }
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
