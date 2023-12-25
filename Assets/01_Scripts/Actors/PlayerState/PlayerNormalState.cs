using System;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

public partial class Player
{
    [Header("Run settings")]
    [SerializeField] private float maxRun = 4f;
    [SerializeField] private float accel = 2.6f;
    [SerializeField] private float accelOnAir = 2.4f;
    [SerializeField] private float deccel = 1.8f;
    
    [Header("Jump/Fall settings")]
    [SerializeField] private float maxFall = -180f;
    [FormerlySerializedAs("jumpStrength")]
    [SerializeField] private float longJumpStrength = 180f;
    [SerializeField] private float shortJumpStrength = 130f;
    [SerializeField] private float gravityValue = 9.8f;
    [SerializeField] private float gravityIncreaseThreshold = 1f;
    [SerializeField] private float CoyoteTime = 0.2f;

    [Header("WallSlide settings")]
    [SerializeField] private float maxSlideFall = -100f;
    [SerializeField] private Vector2 WallJumpSpeed = new Vector2(180, 180);
    [SerializeField] private float WallXMoveLimitTime = 0.2f;
    
    // vars
    private float jumpGraceTimer;
    private float wallSlideTimer;
    private int wallSlideDir;
    private float wallXMoveLimitTimer;
    private float coyoteTimer;
    
    private bool isAtJumpingFrame;

    public int NormalUpdate()
    {
        if (CanDash)
        {
            Speed += LiftBoost;                   
            return StartDash();
        }

        var xDir = inputX;
        if (wallXMoveLimitTimer > 0f && xDir != 0 && Math.Sign(xDir) != Math.Sign(Speed.x))
        {
            xDir = 0;
        }
            
        
        // moveX
        if (Abs(Speed.x) > maxRun)
            Speed.x = MathUtil.Appr(Speed.x, Sign(Speed.x) * maxRun, deccel);
        else
            Speed.x = MathUtil.Appr(Speed.x, xDir * maxRun, onGround ? accel : accelOnAir);
        
        // wall jump
        if (wallDir != 0)
        {
            if (jumpPressed || longJumpPressed)
            {
                Speed = WallJumpSpeed;
                Speed.x *= -wallDir;
                longJumpPressed = false;
                jumpPressed = false;
                
                // 벽방향의 움직임을 잠시 제한해야한다.
                wallXMoveLimitTimer = WallXMoveLimitTime;
            }
        }
        if (jumpBufferTimer <= 0f)
        {
            longJumpPressed = false;
            jumpPressed = false;
        }
        
        //Vertical
        {
            // ground jump
            isAtJumpingFrame = false;
            if (coyoteTimer > 0f)
            {
                Jump();
            }
            
            // gravity
            gravityAccel = gravityValue;
        
            // at the top of jump
            if (Abs(Speed.y) <= gravityIncreaseThreshold)
                gravityAccel *= 0.5f;

            if (!onGround)
            {
                Speed.y = MathUtil.Appr(Speed.y, wallDir == 0 ? maxFall : maxSlideFall, gravityAccel);
            }
        }

        return StateNormal;
    }

    private void Jump()
    {
        // if (checkJumpPressTime)
        // {
        //     if (jumpPressTimer > 0f)
        //     {
        //         jumpPressTimer -= deltaTime;
        //         if (Input.GetKeyUp(KeyCode.C))
        //         {
        //             shortJumpPressed = true;
        //             checkJumpPressTime = false;
        //             jumpBufferTimer = JumpBufferTime;
        //         }
        //     }
        //     else if (Input.GetKey(KeyCode.C))
        //     {
        //         longJumpPressed = true;
        //         checkJumpPressTime = false;
        //         jumpBufferTimer = JumpBufferTime;
        //     }
        // }
        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= deltaTime;

    }
    
    private void SuperJump()
    {
        
    }
}


