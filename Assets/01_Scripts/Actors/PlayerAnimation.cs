
using UnityEngine;
using static UnityEngine.Mathf;

public partial class Player
{
    private Animator anim;
    private SpriteRenderer sr;
    
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

    private readonly int HashIdle = Animator.StringToHash(StAnimIdle);
    private readonly int HashRun = Animator.StringToHash(StAnimRun);
    private readonly int HashReadyJump = Animator.StringToHash(StAnimReadyJump);
    private readonly int HashJumpToFall = Animator.StringToHash(StAnimJumpToFall);
    private readonly int HashJump = Animator.StringToHash(StAnimJump);
    private readonly int HashFall = Animator.StringToHash(StAnimFall);
    private readonly int HashLand = Animator.StringToHash(StAnimLand);
    
    private readonly int HashDash = Animator.StringToHash(StAnimDash);
    
    // vars
    private bool flipAnimFlag = false;
    private bool keepCurrentAnim = false;
    private float landToIdleTimer = 0f;
    private float keepAnimTimer = 0f;
    
    private int curHash;
    private int nextHash;
    private string nextAnim;
    
    private bool hasToSwitch;
    
    private bool facingRight;
    
    private void InitAnimation()
    {
        sr = GetComponent<SpriteRenderer>();
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
        
        if (curHash == HashLand)
        {
            landToIdleTimer = 0.25f;
        }
        landToIdleTimer -= deltaTime;
        
        hasToSwitch = false;
        
        #region Normal State Animations

        if (sm.State == StateNormal)
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
            facingRight = Speed.x > 0;
            sr.flipX = !facingRight ^ flipAnimFlag;
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
        if (curHash == HashIdle)
        {
            flipAnimFlag = true;
            sr.flipX = !sr.flipX;
        }
            
        else if (nextHash == HashIdle)
        {
            flipAnimFlag = false;
            sr.flipX = !sr.flipX;
        }
        
        anim.Play(nextAnim);
    }
}


