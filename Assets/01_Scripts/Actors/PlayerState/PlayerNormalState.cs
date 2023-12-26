using System;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

public partial class Player
{
    [Header("Run settings")]
    [SerializeField] private float MaxRun = 90f;
    [SerializeField] private float RunAccel = 1000f;
    [SerializeField] private float AirMult = 0.65F;
    [SerializeField] private float RunReduce = 400f;
    
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

    private const int WallJumpCheckDist = 3;
    // vars
    private float maxFall;
    
    private float jumpBufferTimer;
    private float jumpGraceTimer;
    private float varJumpTimer;
    private float varJumpSpeed;
    
    private float wallSlideTimer;

    private int forceMoveX;
    private float forceMoveXTimer;

    private void NormalBegin()
    {
        maxFall = MaxFall;
    }
    
    public int NormalUpdate()
    {
        if (CanDash)
        {
            Speed += LiftBoost;                   
            return StartDash();
        }
        
        // Running and Friction
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
                    if (Speed.y <= 0f && wallSlideTimer > 0f && CollideCheck(inputX, 0))
                    {
                        wallSlideDir = (int)facing;
                        if (wallSlideTimer > 0.5f)
                        {
                            dust.Burst(PositionWS, new Vector2(-wallSlideDir, 0), 0.3f);
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
                    if (CollideCheck(1 * WallJumpCheckDist, 0))
                    {
                        WallJump(-1);
                    }
                    else if (CollideCheck(-1 * WallJumpCheckDist, 0))
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
        dust.Burst(PositionWS, Vector2.up, 1);
        
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
        if(dir == -1)
            dust.Burst(CenterWS + Vector2.right * 3, new Vector2(-1, 1), 1);
        else
            dust.Burst(CenterWS + Vector2.right * -3, new Vector2(1, 1), 1);
        
        SaveData.Instance.TotalWallJumps++;
    }
}


