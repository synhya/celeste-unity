using UnityEngine;
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
    [SerializeField] private float jumpStrength = 12f;
    [SerializeField] private float gravityValue = 9.8f;
    [SerializeField] private float gravityIncreaseThreshold = 1f;
    
    // vars
    private float jumpGraceTimer;

    public int NormalUpdate()
    {
        if (CanDash)
        {
            Speed += LiftBoost;                   
            return StartDash();
        }
        
        // move
        if (Abs(Speed.x) > maxRun)
            Speed.x = MathUtil.Appr(Speed.x, Sign(Speed.x) * maxRun, deccel);
        else
            Speed.x = MathUtil.Appr(Speed.x, inputX * maxRun, onGround ? accel : accelOnAir);
        
        // confirm jump
        if (jumpPressed && onGround)
            jumpConfirmed = true;
        else
            jumpConfirmed = false;
        
        if(jumpConfirmed) 
            Speed.y = jumpStrength;
        
        // gravity
        gravityAccel = gravityValue;
        
        // at the top of jump
        if (Abs(Speed.y) <= gravityIncreaseThreshold)
            gravityAccel *= 0.5f;

        if (!onGround)
        {
            Speed.y = MathUtil.Appr(Speed.y, maxFall, gravityAccel);
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


