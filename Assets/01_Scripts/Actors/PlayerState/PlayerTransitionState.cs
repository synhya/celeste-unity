
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

// 레벨에 진입할때의 인트로와
// 룸 이동시의 전환으로 나뉜다.
public partial class Player
{
    [HideInInspector] public Vector2Int TransitionTarget;
    [HideInInspector] public bool ForceFlipXValue = false;
    [HideInInspector] public bool ForceSpriteFlip = false;
    
    [HideInInspector] public bool LockMovementBySpeed;
    
    #region State Transition

    public void TransitionBegin()
    {
        LockMovementBySpeed = true;
        
        RefillDash();
        wallSlideTimer = WallSlideTime;
        forceMoveXTimer = 0;
        jumpGraceTimer = 0; // if falling
        
        var link = Level.NextRoomLink;
        switch (link.Door.Dir)
        {
            case DoorDirections.Up:
                BeforeUpTransition();
                break;
            case DoorDirections.Down:
                BeforeDownTransition();
                break;
            case DoorDirections.Right:
                BeforeSideTransition();
                break;
            case DoorDirections.Left:
                BeforeSideTransition();
                break;
        }
        TransitionTarget = link.Door.TransPosWS;
        // ForceSpriteFlip = true;
        // ForceFlipXValue = TransitionTarget.x < PositionWS.x;
    }

    public int TransitionUpdate()
    {
        // MoveTowardsH(TransitionTarget.x, 60f * Time.deltaTime);
        // MoveTowardsV(TransitionTarget.y, 60f * Time.deltaTime);

        PositionWS = Vector2Int.RoundToInt(Vector2.MoveTowards(PositionWS, TransitionTarget, 140 * Time.deltaTime));

        // set speed just for animation
        Speed = TransitionTarget - PositionWS;
        
        if (PositionWS == TransitionTarget)
        {
            // no more extra moves.
            Remainder.x = 0;
            Remainder.y = 0;
            Speed.x = Mathf.Round(Speed.x);
            Speed.y = Mathf.Round(Speed.y);
            return StateNormal;
        }

        return StateTransition;
    }

    public void TransitionEnd()
    {
        LockMovementBySpeed = false;
        ForceSpriteFlip = false;
        Level.OnTransitionEnd();
    }

    public void BeforeSideTransition()
    {
        
    }

    public void BeforeDownTransition()
    {
        sm.State = StateNormal;

        Speed.y = Mathf.Max(0, Speed.y);
        varJumpTimer = 0;
    }

    public void BeforeUpTransition()
    {
        Speed.x = 0;

        Speed.y = varJumpSpeed = JumpSpeed;
        varJumpTimer = VarJumpTime;
        
        sm.State = StateNormal; // if was dashing. have to stop x movement
        dashCoolDownTimer = DashCoolDown; 
    }

    #endregion
    
    #region Level Intros
    
    private void IntroJumpBegin()
    {
        Collideable = false;
        
        Y = Room.BoundRect.y - 16;
        X = Room.SpawnPosWS.x; // later change to move diagonal

        // after update before late update
        StartCoroutine(IntroJumpCoroutine());
    }

    private int IntroJumpUpdate() => StateIntroJump;

    private IEnumerator IntroJumpCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        // move up
        {
            // move with force
            while (Y < Room.SpawnPosWS.y + 8)
            {
                Speed.y = 120;
                yield return null;
            }
            Collideable = true;
        }
        
        // slow down
        {
            while (Speed.y > 0)
            {
                Speed.y -= 800 * Time.deltaTime;
                yield return null;
            }
            Speed.y = 0; // stop at top
        }
        
        // fall down
        {
            while (!onGround)
            {
                Speed.y -= 800 * Time.deltaTime;
                yield return null;
            }
        }
        
        // land
        {
            EffectManager.ShakeCam(0.6f, 1.2f);
        }
        
        sm.State = StateNormal;
    }

    private void IntroJumpEnd()
    {
        Level.OnPlayerIntroComplete.Invoke();
    }

    #endregion
}


