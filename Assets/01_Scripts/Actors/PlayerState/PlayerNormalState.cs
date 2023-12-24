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
        
        // ground jump
        isAtJumpingFrame = false;
        if (onGround || coyoteTimer > 0f)
        {
            if(shortJumpPressed) 
            {
                Speed.y = shortJumpStrength;
                shortJumpPressed = false;
                isAtJumpingFrame = true;
            }
            else if (longJumpPressed)
            {
                Speed.y = longJumpStrength;
                longJumpPressed = false;
                isAtJumpingFrame = true;
            }
        }
        // wall jump
        else if (wallDir != 0)
        {
            if (shortJumpPressed || longJumpPressed)
            {
                Speed = WallJumpSpeed;
                Speed.x *= -wallDir;
                longJumpPressed = false;
                shortJumpPressed = false;
                
                // 벽방향의 움직임을 잠시 제한해야한다.
                wallXMoveLimitTimer = WallXMoveLimitTime;
            }
        }
        if (jumpBufferTimer <= 0f)
        {
            longJumpPressed = false;
            shortJumpPressed = false;
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

        return StateNormal;
    }

    private void Jump()
    {
        
    }
    
    private void SuperJump()
    {
        
    }
}


