using System;
using System.Collections;
using UnityEngine;


public class Player : Actor
{
    public const int StNormal = 0;
    public const int StClimb = 1;
    public const int StDash = 2;
    public const int StSwim = 3;
    public const int StBoost = 4;
    public const int StRedDash = 5;
    public const int StHitSquash = 6;
    public const int StLaunch = 7;
    public const int StPickup = 8;
    public const int StDreamDash = 9;
    public const int StSummitLaunch = 10;
    // public const int StDummy = 11;
    // public const int StIntroWalk = 12;
    // public const int StIntroJump = 13;
    // public const int StIntroRespawn = 14;
    // public const int StIntroWakeUp = 15;
    // public const int StBirdDashTutorial = 16;
    public const int StFrozen = 17;
    public const int StReflectionFall = 18;
    public const int StStarFly = 19;
    public const int StTempleFall = 20;
    public const int StCassetteFly = 21;
    public const int StAttract = 22;


    #region vars

    public Animator Animator;
    public StateMachine StateMachine;

    public Vector2 PreviousPosition;

    private float idleTimer;

    #endregion

    protected override void Start()
    {
        base.Start();
        
        StateMachine = StateMachine.AttachStateMachine(this.gameObject, 23);
        
        StateMachine.SetCallbacks(StNormal, NormalUpdate, null, NormalBegin, NormalEnd);
        StateMachine.SetCallbacks(StDash, null, DashCoroutine(), null, null);
        
    }

    #region Updating

    public void Update()
    {
        PreviousPosition = Position;
        
        //Vars
        {
            // strawb reset timer
            
            // idle timer
            idleTimer += Time.deltaTime;
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
