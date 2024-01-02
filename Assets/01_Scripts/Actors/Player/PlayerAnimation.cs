
using UnityEngine;
using static UnityEngine.Mathf;

public partial class Player
{
    private Animator anim;
    
    [Header("Animation Settings")]
    [SerializeField] private float runAnimThreshold = 1f;
    [SerializeField] private float jumpTurnAroundThreshold = 10f;
    
    // anim const
    private const string StAnimIdle = "Player_Idle";
    private const string StAnimRun = "Player_Run";
    private const string StAnimReadyJump = "Player_ReadyJump";
    private const string StAnimJump = "Player_Jump";
    private const string StAnimJumpToFall = "Player_JumpToFall";
    private const string StAnimFall = "Player_Fall";
    private const string StAnimLand = "Player_Land";
    
    private const string StAnimDash = "Player_Dash";
    private const string StAnimWallSlide = "Player_WallSlide";

    private const string StAnimDucking = "Player_Duck";
    private const string StAnimUnDucking = "Player_UnDuck";

    private readonly int HashIdle = Animator.StringToHash(StAnimIdle);
    private readonly int HashRun = Animator.StringToHash(StAnimRun);
    private readonly int HashReadyJump = Animator.StringToHash(StAnimReadyJump);
    private readonly int HashJumpToFall = Animator.StringToHash(StAnimJumpToFall);
    private readonly int HashJump = Animator.StringToHash(StAnimJump);
    private readonly int HashFall = Animator.StringToHash(StAnimFall);
    private readonly int HashLand = Animator.StringToHash(StAnimLand);
    
    private readonly int HashDash = Animator.StringToHash(StAnimDash);
    private readonly int HashWallSlide = Animator.StringToHash(StAnimWallSlide);
    
    private readonly int HashDucking = Animator.StringToHash(StAnimDucking);
    private readonly int HashUnDucking = Animator.StringToHash(StAnimUnDucking);
    
    // vars
    private bool flipAnimFlag = false;
    private bool keepCurrentAnim = false;
    private float keepAnimTimer = 0f;
    
    private int curHash;
    private int nextHash;
    private string nextAnim;
    
    private bool hasToSwitch;
    
    private void InitAnimation()
    {
        anim = GetComponent<Animator>();
    }

    private void SetAnimation()
    {
        if (keepAnimTimer > 0f)
        {
            keepAnimTimer -= deltaTime;
            return;
        }
        
        // get current state
        var currentAnim = anim.GetCurrentAnimatorStateInfo(0);
        curHash = currentAnim.shortNameHash;
        
        hasToSwitch = false;
        
        
        if(curHash != HashDucking && !wasDucking && Ducking)
        {
            SetAsNextAnimation(HashDucking, StAnimDucking);
        }
        else if (curHash != HashUnDucking && wasDucking && !Ducking)
        {
            SetAsNextAnimation(HashUnDucking, StAnimUnDucking);
        }
        else if ((curHash == HashDucking && Ducking) || (curHash == HashUnDucking && currentAnim.normalizedTime <= 0.95f))
        {
            // keep 
        }
        #region Normal State Animations

        else if (sm.State == StateNormal)
        {
            if (wallSlideDir != 0)
            {
                if(curHash != HashWallSlide)
                    SetAsNextAnimation(HashWallSlide, StAnimWallSlide);
            }
            else
            {
                if ((curHash != HashLand && curHash != HashIdle && onGround && Abs(Speed.x) <= runAnimThreshold)
                    || (curHash == HashLand && currentAnim.normalizedTime > 0.95f))
                    SetAsNextAnimation(HashIdle, StAnimIdle);
                else if (curHash != HashRun &&
                         onGround && Abs(Speed.x) > runAnimThreshold)
                    SetAsNextAnimation(HashRun, StAnimRun);
            
                // else if (curState.shortNameHash != HashReadyJump &&
                //          jumpConfirmed)
                // {
                //     shouldKeepCurrentAnimation = true;
                //     SwitchAnimation(HashReadyJump, StAnimReadyJump);
                // }
        
                else if (curHash != HashJump &&
                         !onGround && Speed.y > jumpTurnAroundThreshold)
                    SetAsNextAnimation(HashJump, StAnimJump);
        
                else if (curHash != HashJumpToFall &&
                         !onGround && Abs(Speed.y) <= jumpTurnAroundThreshold)
                    SetAsNextAnimation(HashJump, StAnimJumpToFall);
        
                else if (curHash != HashFall &&
                         !onGround && Speed.y < -jumpTurnAroundThreshold)
                    SetAsNextAnimation(HashFall, StAnimFall);
        
                if (curHash == HashFall &&
                    !wasGround && onGround)  // 점프 시간재서 일정시간 이상이면 플레이하게 바꾸자.
                    SetAsNextAnimation(HashLand, StAnimLand);
            }
        }
            

        #endregion

        #region Dash State Animations

        else if (sm.State == StateDash)
        {
            if(curHash != HashDash)
                SetAsNextAnimation(HashDash, StAnimDash);
        }

        #endregion
        
        // attack
        
        // ...
        
        // finally switch
        if(hasToSwitch)
            SwitchAnimation();
        
        // after switching animation
        if (Speed.x != 0)
        {
            facing = Speed.x > 0 ? Facings.Right : Facings.Left;
            SR.flipX = (facing == Facings.Left) ^ flipAnimFlag;
        }
        
        
        if (keepCurrentAnim)
        {
            keepCurrentAnim = false;
            keepAnimTimer = currentAnim.length;
        }
    }

    void SetAsNextAnimation(int nextHash ,string nextAnim)
    {
        hasToSwitch = true;
        this.nextHash = nextHash;
        this.nextAnim = nextAnim;
    }

    void SwitchAnimation()
    {
        if (curHash == HashIdle || curHash == HashWallSlide)
        {
            flipAnimFlag = true;
            SR.flipX = !SR.flipX;
        }
            
        else if (nextHash == HashIdle || curHash == HashWallSlide)
        {
            flipAnimFlag = false;
            SR.flipX = !SR.flipX;
        }
        
        if(gameObject.activeSelf)
            anim.Play(nextAnim);
    }

    #region original script

    // private void UpdateSprite()
    // {
    //     //Tween
    //     Sprite.Scale.X = Calc.Approach(Sprite.Scale.X, 1f, 1.75f * Engine.DeltaTime);
    //     Sprite.Scale.Y = Calc.Approach(Sprite.Scale.Y, 1f, 1.75f * Engine.DeltaTime);
    //
    //     //Animation
    //     if (InControl && Sprite.CurrentAnimationID != PlayerSprite.Throw && StateMachine.State != StTempleFall && 
    //         StateMachine.State != StReflectionFall && StateMachine.State != StStarFly && StateMachine.State != StCassetteFly)
    //     {
    //         if (StateMachine.State == StAttract)
    //         {
    //             Sprite.Play(PlayerSprite.FallFast);
    //         }
    //         else if (StateMachine.State == StSummitLaunch)
    //         {
    //             Sprite.Play(PlayerSprite.Launch);
    //         }
    //         // picking up
    //         else if (StateMachine.State == StPickup)
    //         {
    //             Sprite.Play(PlayerSprite.PickUp);
    //         }
    //         // swiming
    //         else if (StateMachine.State == StSwim)
    //         {
    //             if (Input.MoveY.Value > 0)
    //                 Sprite.Play(PlayerSprite.SwimDown);
    //             else if (Input.MoveY.Value < 0)
    //                 Sprite.Play(PlayerSprite.SwimUp);
    //             else
    //                 Sprite.Play(PlayerSprite.SwimIdle);
    //         }
    //         // dream dashing
    //         else if (StateMachine.State == StDreamDash)
    //         {
    //             if (Sprite.CurrentAnimationID != PlayerSprite.DreamDashIn && Sprite.CurrentAnimationID != PlayerSprite.DreamDashLoop)
    //                 Sprite.Play(PlayerSprite.DreamDashIn);
    //         }
    //         else if (Sprite.DreamDashing && Sprite.LastAnimationID != PlayerSprite.DreamDashOut)
    //         {
    //             Sprite.Play(PlayerSprite.DreamDashOut);
    //         }
    //         else if (Sprite.CurrentAnimationID != PlayerSprite.DreamDashOut)
    //         {
    //             // during dash
    //             if (DashAttacking)
    //             {
    //                 if (onGround && DashDir.Y == 0 && !Ducking && Speed.X != 0 && moveX == -Math.Sign(Speed.X))
    //                 {
    //                     if (Scene.OnInterval(.02f))
    //                         Dust.Burst(Position, Calc.Up, 1);
    //                     Sprite.Play(PlayerSprite.Skid);
    //                 }
    //                 else
    //                     Sprite.Play(PlayerSprite.Dash);
    //             }
    //             // climbing
    //             else if (StateMachine.State == StClimb)
    //             {
    //                 if (lastClimbMove < 0)
    //                     Sprite.Play(PlayerSprite.ClimbUp);
    //                 else if (lastClimbMove > 0)
    //                     Sprite.Play(PlayerSprite.WallSlide);
    //                 else if (!CollideCheck<Solid>(Position + new Vector2((int)Facing, 6)))
    //                     Sprite.Play(PlayerSprite.Dangling);
    //                 else if (Input.MoveX == -(int)Facing)
    //                 {
    //                     if (Sprite.CurrentAnimationID != PlayerSprite.ClimbLookBack)
    //                         Sprite.Play(PlayerSprite.ClimbLookBackStart);
    //                 }
    //                 else
    //                     Sprite.Play(PlayerSprite.WallSlide);
    //             }
    //             // ducking
    //             else if (Ducking && StateMachine.State == StNormal)
    //             {
    //                 Sprite.Play(PlayerSprite.Duck);
    //             }
    //             else if (onGround)
    //             {
    //                 fastJump = false;
    //                 if (Holding == null && moveX != 0 && CollideCheck<Solid>(Position + Vector2.UnitX * moveX))
    //                 {
    //                     Sprite.Play("push");
    //                 }
    //                 else if (Math.Abs(Speed.X) <= RunAccel / 40f && moveX == 0)
    //                 {
    //                     if (Holding != null)
    //                     {
    //                         Sprite.Play(PlayerSprite.IdleCarry);
    //                     }
    //                     else if (!Scene.CollideCheck<Solid>(Position + new Vector2((int)Facing * 1, 2)) && !Scene.CollideCheck<Solid>(Position + new Vector2((int)Facing * 4, 2)) && !CollideCheck<JumpThru>(Position + new Vector2((int)Facing * 4, 2)))
    //                     {
    //                         Sprite.Play(PlayerSprite.FrontEdge);
    //                     }
    //                     else if (!Scene.CollideCheck<Solid>(Position + new Vector2(-(int)Facing * 1, 2)) && !Scene.CollideCheck<Solid>(Position + new Vector2(-(int)Facing * 4, 2)) && !CollideCheck<JumpThru>(Position + new Vector2(-(int)Facing * 4, 2)))
    //                     {
    //                         Sprite.Play("edgeBack");
    //                     }
    //                     else if (Input.MoveY.Value == -1)
    //                     {
    //                         if (Sprite.LastAnimationID != PlayerSprite.LookUp)
    //                             Sprite.Play(PlayerSprite.LookUp);
    //                     }
    //                     else
    //                     {
    //                         if (Sprite.CurrentAnimationID != null && !Sprite.CurrentAnimationID.Contains("idle"))
    //                             Sprite.Play(PlayerSprite.Idle);
    //                     }
    //                 }
    //                 else if (Holding != null)
    //                 {
    //                     Sprite.Play(PlayerSprite.RunCarry);
    //                 }
    //                 else if (Math.Sign(Speed.X) == -moveX && moveX != 0)
    //                 {
    //                     if (Math.Abs(Speed.X) > MaxRun)
    //                         Sprite.Play(PlayerSprite.Skid);
    //                     else if (Sprite.CurrentAnimationID != PlayerSprite.Skid)
    //                         Sprite.Play(PlayerSprite.Flip);
    //                 }
    //                 else if (windDirection.X != 0 && windTimeout > 0f && (int)Facing == -Math.Sign(windDirection.X))
    //                 {
    //                     Sprite.Play(PlayerSprite.RunWind);
    //                 }
    //                 else if (!Sprite.Running)
    //                 {
    //                     if (Math.Abs(Speed.X) < MaxRun * .5f)
    //                         Sprite.Play(PlayerSprite.RunSlow);
    //                     else
    //                         Sprite.Play(PlayerSprite.RunFast);
    //                 }
    //             }
    //             // wall sliding
    //             else if (wallSlideDir != 0 && Holding == null)
    //             {
    //                 Sprite.Play(PlayerSprite.WallSlide);
    //             }
    //             // jumping up
    //             else if (Speed.Y < 0)
    //             {
    //                 if (Holding != null)
    //                 {
    //                     Sprite.Play(PlayerSprite.JumpCarry);
    //                 }
    //                 else if (fastJump || Math.Abs(Speed.X) > MaxRun)
    //                 {
    //                     fastJump = true;
    //                     Sprite.Play(PlayerSprite.JumpFast);
    //                 }
    //                 else
    //                     Sprite.Play(PlayerSprite.JumpSlow);
    //             }
    //             // falling down
    //             else
    //             {
    //                 if (Holding != null)
    //                 {
    //                     Sprite.Play(PlayerSprite.FallCarry);
    //                 }
    //                 else if (fastJump || Speed.Y >= MaxFall || level.InSpace)
    //                 {
    //                     fastJump = true;
    //                     if (Sprite.LastAnimationID != PlayerSprite.FallFast)
    //                         Sprite.Play(PlayerSprite.FallFast);
    //                 }
    //                 else
    //                     Sprite.Play(PlayerSprite.FallSlow);
    //             }
    //         }
    //     }
    //
    //     if (StateMachine.State != Player.StDummy)
    //     {
    //         if (level.InSpace)
    //             Sprite.Rate = .5f;
    //         else
    //             Sprite.Rate = 1f;
    //     }
    // }

    #endregion
}


