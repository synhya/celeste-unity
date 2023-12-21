
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

    private readonly int HashIdle = Animator.StringToHash(StAnimIdle);
    private readonly int HashRun = Animator.StringToHash(StAnimRun);
    private readonly int HashReadyJump = Animator.StringToHash(StAnimReadyJump);
    private readonly int HashJumpToFall = Animator.StringToHash(StAnimJumpToFall);
    private readonly int HashJump = Animator.StringToHash(StAnimJump);
    private readonly int HashFall = Animator.StringToHash(StAnimFall);
    private readonly int HashLand = Animator.StringToHash(StAnimLand);

    private bool flipAnimFlag = false;
    private bool shouldKeepCurrentAnimation = false;
    private float keepAnimTimer = 0f;

    private void InitAnimation()
    {
        anim = GetComponent<Animator>();
    }

    private void SetAnimation()
    {
        if (keepAnimTimer > 0f)
        {
            keepAnimTimer -= Time.deltaTime;
            return;
        }
        
        // get current state
        var curState = anim.GetCurrentAnimatorStateInfo(0);
        
        if (shouldKeepCurrentAnimation)
        {
            shouldKeepCurrentAnimation = false;

            keepAnimTimer = curState.length - Time.deltaTime;
            return;
        }
        
        // figure out what should be current animation based on state
        if (curState.shortNameHash != HashIdle &&
            onGround && Abs(Speed.x) <= runAnimThreshold)
            SwitchAnimation(curState.shortNameHash, HashIdle, StAnimIdle);
        else if (curState.shortNameHash != HashRun &&
                 onGround && Abs(Speed.x) > runAnimThreshold)
            SwitchAnimation(curState.shortNameHash, HashRun, StAnimRun);
        
        
        // ready jump turnaround fall land
        #region OnAir

        // else if (curState.shortNameHash != HashReadyJump &&
        //          jumpConfirmed)
        // {
        //     shouldKeepCurrentAnimation = true;
        //     SwitchAnimation(curState.shortNameHash, HashReadyJump, StAnimReadyJump);
        // }
        
        else if (curState.shortNameHash != HashJump &&
                 !onGround && Speed.y > jumpTurnAroundThreshold)
            SwitchAnimation(curState.shortNameHash, HashJump, StAnimJump);
        
        else if (curState.shortNameHash != HashJumpToFall &&
                 !onGround && Abs(Speed.y) <= jumpTurnAroundThreshold)
            SwitchAnimation(curState.shortNameHash, HashJump, StAnimJumpToFall);
        
        else if (curState.shortNameHash != HashFall &&
                 !onGround && Speed.y < -jumpTurnAroundThreshold)
            SwitchAnimation(curState.shortNameHash, HashFall, StAnimFall);
        
        else if (curState.shortNameHash != HashLand &&
                 !wasGround && onGround  && Speed.y > jumpTurnAroundThreshold)
        {
            Debug.Log("Land");
            shouldKeepCurrentAnimation = true;
            SwitchAnimation(curState.shortNameHash, HashLand, StAnimLand);
        }

        #endregion
        
        // attack
        
        
        // set flip
        if (Speed.x != 0)
            sr.flipX = Speed.x < 0 ^ flipAnimFlag;
    }

    void SwitchAnimation(int curHash, int nextHash ,string nextAnim)
    {
        if (curHash == HashIdle)
            flipAnimFlag = true;
        else if (nextHash == HashIdle)
        {
            flipAnimFlag = false;
            sr.flipX = !sr.flipX;
        }
        
        anim.Play(nextAnim);
    }
}


