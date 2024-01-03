
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

    public void TransitionBegin()
    {
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
    

    #region Level Intros
    
    // private Facings introWalkDirection;
    // private IntroTypes IntroType { get; set; }
    //
    // public override void Added(Level level)
    // {
    //     IntroType = level.IntroType;
    //     switch (IntroType)
    //     {
    //         case IntroTypes.Respawn:
    //             sm.State = StateNormal;
    //             break;
    //         case IntroTypes.WalkInRight:
    //             introWalkDirection = Facings.Right;
    //             sm.State = StateIntroWalk;
    //             break;
    //         case IntroTypes.WalkInLeft:
    //             introWalkDirection = Facings.Left;
    //             sm.State = StateIntroWalk;
    //             break;
    //         case IntroTypes.Jump:
    //             sm.State = StateIntroJump;
    //             break;
    //         case IntroTypes.None:
    //             sm.State = StateNormal;
    //             break;
    //         case IntroTypes.Fall:
    //             sm.State = StateReflectionFall;
    //             break;
    //     }
    //     IntroType = IntroTypes.Transition;
    // }
    
    private void BeginIntroJump()
    {
        StartCoroutine(IntroJumpCoroutine());
    }

    private IEnumerator IntroJumpCoroutine()
    {
        var start = PositionWS;
        
        

        return null;
    }

    #endregion
}


