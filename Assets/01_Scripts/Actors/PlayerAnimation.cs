
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
            keepAnimTimer -= Time.deltaTime;
            return;
        }
        
        // get current state
        var curState = anim.GetCurrentAnimatorStateInfo(0);
        hasToSwitch = false;
        
        // figure out what should be current animation based on state
        if (curState.shortNameHash != HashIdle &&
            onGround && Abs(Speed.x) <= runAnimThreshold)
            SetAsNextAnimation(curState.shortNameHash, HashIdle, StAnimIdle);
        else if (curState.shortNameHash != HashRun &&
                 onGround && Abs(Speed.x) > runAnimThreshold)
            SetAsNextAnimation(curState.shortNameHash, HashRun, StAnimRun);
        
        
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
            SetAsNextAnimation(curState.shortNameHash, HashJump, StAnimJump);
        
        else if (curState.shortNameHash != HashJumpToFall &&
                 !onGround && Abs(Speed.y) <= jumpTurnAroundThreshold)
            SetAsNextAnimation(curState.shortNameHash, HashJump, StAnimJumpToFall);
        
        else if (curState.shortNameHash != HashFall &&
                 !onGround && Speed.y < -jumpTurnAroundThreshold)
            SetAsNextAnimation(curState.shortNameHash, HashFall, StAnimFall);
        
        if (curState.shortNameHash == HashFall &&
                 !wasGround && onGround)  // 점프 시간재서 일정시간 이상이면 플레이하게 바꾸자.
        {
            shouldKeepCurrentAnimation = true;
            SetAsNextAnimation(curState.shortNameHash, HashLand, StAnimLand);
        }

        #endregion
        
        // attack
        
        // ...
        
        // finally switch
        if(hasToSwitch)
            SwitchAnimation();
        
        // after switching animation
        if (Speed.x != 0)
            sr.flipX = Speed.x < 0 ^ flipAnimFlag;
        
        if (shouldKeepCurrentAnimation)
        {
            shouldKeepCurrentAnimation = false;

            keepAnimTimer = curState.length;
        }
    }

    void SetAsNextAnimation(int curHash, int nextHash ,string nextAnim)
    {
        hasToSwitch = true;
        this.curHash = curHash;
        this.nextHash = nextHash;
        this.nextAnim = nextAnim;
    }

    void SwitchAnimation()
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


