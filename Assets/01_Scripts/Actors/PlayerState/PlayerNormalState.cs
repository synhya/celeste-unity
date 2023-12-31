using System;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

public partial class Player
{
    [Header("Horizontal Move settings")]
    [SerializeField] private float MaxRun = 90f;
    [SerializeField] private float RunAccel = 1000f;
    [SerializeField] private float AirMult = 0.65F;
    [SerializeField] private float RunReduce = 400f;
    [SerializeField] private float DuckFriction = 500f;
    
    [Header("Jump settings")]
    [SerializeField] private float JumpHBoost = 40f;
    [SerializeField] private float WallJumpHBoost = 130f;
    [SerializeField] private float JumpSpeed = 105f;
    [SerializeField] private float VarJumpTime = 0.2f;
    [SerializeField] private float JumpBufferTime = 0.1f;
    [SerializeField] private float JumpGraceTime = 0.15f;
    
    [Header("Fall settings")]
    [SerializeField] private float MaxFall = -160f;
    [SerializeField] private float FastMaxFall = -240f;
    [SerializeField] private float MaxFallAccel = 300f;
    
    [SerializeField] private float Gravity = 900f;

    [Header("WallSlide settings")]
    [SerializeField] private float WallSlideStartMaxFall = -20f;
    [SerializeField] private Vector2 WallJumpSpeed = new Vector2(180, 180);
    [SerializeField] private float WallJumpForceTime = 0.16f;
    [SerializeField] private float WallSlideTime = 1.2f;
    
    [Header("Lift Boost settings")]
    [SerializeField] private float LiftYCap = 130f;
    [SerializeField] private float LiftXCap = 250f;

    private const int WallJumpCheckDist = 3;
    private const int NormalHitboxY = 15;
    private const int DuckHitboxY = 9;
    
    // vars
    private float maxFall;
    
    private float jumpBufferTimer;
    private float jumpGraceTimer;
    private float varJumpTimer;
    private float varJumpSpeed;
    
    private float wallSlideTimer;

    private int forceMoveX;
    private float forceMoveXTimer;
    private float wallSlideParticleTimer;

    private bool Ducking
    {
        get => HitboxSize.y == DuckHitboxY;
        // may also have hurt box with different size.
        set => HitboxSize.y = value ? DuckHitboxY : NormalHitboxY;
    }

    private bool CanUnDuck
    {
        get {
            if (!Ducking) return true;

            var was = HitboxSize.y;
            HitboxSize.y = NormalHitboxY;
            bool ret = !CollideCheck(Vector2Int.zero);

            HitboxSize.y = was;
            return ret;
        }
    }

    private Vector2 LiftBoost
    {
        get {
            Vector2 val = LiftSpeed;

            if (Abs(val.x) > LiftXCap)
                val.x = LiftXCap * Sign(val.x);

            if (val.y < 0) val.y = 0;
            else val.y = Min(val.y, LiftYCap);

            return val;
        }
    }

    private void NormalBegin()
    {
        maxFall = MaxFall;
    }
    
    public int NormalUpdate()
    {
        // 올라가는 플랫폼에서 떨어졌을때 속도 전달
        if (LiftBoost.y > 0 && wasGround && !onGround && Speed.y <= 0)
            Speed.y = LiftBoost.y;
        
        // Dashing
        if (CanDash)
        {
            Speed += LiftBoost;                   
            return StartDash();
        }
        
        // Ducking 
        if (Ducking)
        {
            if ( !onGround || (inputY != -1 && CanUnDuck))
            {
                Ducking = false;
            }
        }
        else if (onGround && inputY == -1 && Speed.y <= 0)
        {
            Ducking = true;
        }
        
        // Running and Friction
        if (Ducking)
        {
            Speed.x = MoveTowards(Speed.x, 0, DuckFriction * deltaTime);
        }
        else
        {
            float mult = onGround ? 1 : AirMult;
            // if (onGround && level.CoreMode == Session.CoreModes.Cold) mult *= .3f;

            float max = MaxRun;
            if (Abs(Speed.x) > max && (int)Sign(Speed.x) == inputX)
                Speed.x = MoveTowards(Speed.x, max * inputX, RunReduce * mult * deltaTime);
            else
                Speed.x = MoveTowards(Speed.x, max * inputX, RunAccel * mult * deltaTime);
        }
        
        //Vertical
        {
            //Calculate current max fall speed
            {
                float mf = MaxFall;
                float fmf = FastMaxFall;
                
                //Fast Fall
                if (inputY == -1 && Speed.y >= mf)
                {
                    maxFall = MoveTowards(maxFall, fmf, MaxFallAccel * deltaTime);

                    float half = mf + (fmf - mf) * .5f;
                    if (Speed.y >= half)
                    {
                        // sprite scale effect
                    }
                }
                else
                {
                    maxFall = MoveTowards(maxFall, mf, MaxFallAccel * deltaTime);
                }
            }
            
            //Gravity
            if (!onGround)
            {
                var max = MaxFall;

                //Wall Slide
                if (inputX == (int)facing)
                {
                    if (Speed.y <= 0f && wallSlideTimer > 0f && CollideCheck(Vector2Int.right * inputX))
                    {
                        wallSlideDir = (int)facing;
                        if (wallSlideTimer > 0.5f)
                        {
                            if (wallSlideDir == 1 && wallSlideParticleTimer <= 0f)
                            {
                                wallSlideParticleTimer = 0.4f;
                                Dust.Burst(BottonRightWS + Vector2.up * 2,
                                    new Vector2(2, 5), new Vector2(-1f, 1f), 0.6f);
                            }
                            else if (wallSlideParticleTimer <= 0f)
                            {
                                wallSlideParticleTimer = 0.4f;
                                Dust.Burst(BottonLeftWS + Vector2.up * 2,
                                    new Vector2(2, 5), new Vector2(1f, 1f), 0.6f);
                            }
                        }
                    }
                        

                    if (wallSlideDir != 0)
                    {
                        max = Lerp(MaxFall, WallSlideStartMaxFall, wallSlideTimer / WallSlideTime);
                    }
                }

                Speed.y = MoveTowards(Speed.y, max, Gravity * deltaTime);
            }
            
            //Variable Jumping
            if (varJumpTimer > 0f)
            {
                if (jumpPressing)
                    Speed.y = Max(Speed.y, varJumpSpeed);
                else
                    varJumpTimer = 0f;
            }
            
            //Jumping
            if (jumpBufferTimer > 0f)
            {
                if (jumpGraceTimer > 0f)
                {
                    Jump();
                }
                else // else if (CanUnDuck) 
                {
                    if (CollideCheck(Vector2Int.right * WallJumpCheckDist))
                    {
                        WallJump(-1);
                    }
                    else if (CollideCheck(Vector2Int.left * WallJumpCheckDist))
                    {
                        WallJump(1);
                    }
                }
            }
        }

        return StateNormal;
    }



    private void Jump()
    {
        jumpGraceTimer = 0;
        jumpBufferTimer = 0;

        varJumpTimer = VarJumpTime;
        // dashAttackTimer = 0;
        wallSlideTimer = WallSlideTime;
        // wallBoostTimer = 0;

        Speed.x += JumpHBoost * inputX;
        Speed.y = JumpSpeed;
        Speed += LiftBoost;
        varJumpSpeed = Speed.y;
        
        // play sfx
        // if (playSfx)
        // {
        //     if (launched)
        //         Play(Sfxs.char_mad_jump_assisted);
        //
        //     if (dreamJump)
        //         Play(Sfxs.char_mad_jump_dreamblock);
        //     else
        //         Play(Sfxs.char_mad_jump);
        // }

        // transform.localScale = new Vector3(.6f, 1.4f, 1f); -> rollback when landing
        
        // play patrticles
        Dust.Burst(PositionWS + Vector2.up, new Vector2(4, 2), 
            Vector2.up + Vector2.right * inputX, 1);
        
        SaveData.Instance.TotalJumps++;
    }

    private void WallJump(int dir)
    {
        jumpGraceTimer = 0;
        jumpBufferTimer = 0;
        
        varJumpTimer = VarJumpTime;
        // dashAttackTimer = 0;
        wallSlideTimer = WallSlideTime;
        // wallBoostTimer = 0;
        if (inputX != 0)
        {
            forceMoveX = dir;
            forceMoveXTimer = WallJumpForceTime;
        }
        
        //Get list of wall jumped off of
        // -> 움직이는 플렛폼의 스피드를 이어받아야 할 경우ㅡ
        
        
        Speed.x += WallJumpHBoost * dir;
        Speed.y = JumpSpeed;
        Speed += LiftBoost;
        varJumpSpeed = Speed.y;
        
        
        // play patrticles
        if (dir == -1)
            Dust.Burst(CenterRightWS + Vector2.left,
                new Vector2(2,3), new Vector2(dir, 1), 1f);
        else
            Dust.Burst(CenterLeftWS + Vector2.right,
                new Vector2(2,3), new Vector2(dir, 1), 1f);
        
        SaveData.Instance.TotalWallJumps++;
    }
}


