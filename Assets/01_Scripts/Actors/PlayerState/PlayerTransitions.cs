
using System;
using System.Collections;
using UnityEngine;

// 레벨에 진입할때의 인트로와
// 룸 이동시의 전환으로 나뉜다.
public partial class Player
{

    /// <summary>
    /// call on room function.
    /// </summary>
    #region Room Transitions

    private void OnTransition()
    {
        RefillDash();
        wallSlideTimer = WallSlideTime;
        forceMoveXTimer = 0;
        jumpGraceTimer = 0; // if falling

        // Leader.TransferFollowers(); -> strawberry movement. 
    }
    
    public bool TransitionsTo(Vector2Int target, Vector2 direction)
    {
        MoveTowardsH(target.x, 60f);
        MoveTowardsV(target.y, 60f);

        if (PositionWS == target)
        {
            // no more extra moves.
            Remainder.x = 0;
            Remainder.y = 0;
            Speed.x = Mathf.Round(Speed.x);
            Speed.y = Mathf.Round(Speed.y);
            return true;
        }
        return false;
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


