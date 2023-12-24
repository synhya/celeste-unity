
using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

public partial class Player
{
    [Header("Dash Settings")]
    [SerializeField] private float DashSpeed = 220f;
    [SerializeField] private float DashYRatio = 0.6f;
    private const float EndDashSpeed = 160f; // end speed?
    private const float EndDashUpMult = .75f;
    private const float DashTime = .15f;
    private const float DashCoolDown = .2f;
    private const float DashRefillCooldown = .1f;
    private const int DashHJumpThruNudge = 6; // ?
    private const int DashCornerCorrection = 4;
    private const int DashVFloorSnapDist = 3; // vertical snap
    private const float DashAttackTime = .3f; // Attack? => brake hidden wall


    [SerializeField] private float[] DashTrailTimeArray;
    [SerializeField] private GameObject playerTrailPrefab;
    

    // vars    
    private bool dashPressed;

    [HideInInspector] public int MaxDashes = 1;
    [HideInInspector] public int Dashes;
    [HideInInspector] public Vector2? OverrideDashDirection;

    private float dashTimer;
    private float dashCoolDownTimer;
    private float dashRefillCooldownTimer;
    [HideInInspector] public Vector2 DashDir;
    
    private bool dashStartedOnGround;
    
    private int lastDashes;

    private float dashTrailTimer;
    private int trailsLeft = 0;

    // use shader?
    public static readonly Color NormalHairColor = new Color(0.67f, 0.2f, 0.2f);
    public static readonly Color UsedHairColor = new Color(0.27f, 0.72f, 1f);
    public static readonly Color FlashHairColor = Color.white;

    private float hairFlashTimer;
    public Color? OverrideHairColor;
    
    private Vector2 beforeDashSpeed;
    private bool wasDashB;

    /// <summary>
    /// Platform lift value ( leave 0 for now )
    /// </summary>
    private Vector2 LiftBoost
    {
        get
        {
            return Vector2.zero;
        }
    }

    public int StartDash()
    {
        wasDashB = Dashes == 2;
        Dashes = Max(0, Dashes - 1);
        return StateDash;
    }
    
    public bool CanDash
    {
        get
        {
            return dashPressed && dashCoolDownTimer <= 0 && Dashes > 0;
        }
    }

    #region StateBehavior

    private void CallDashEvents()
    {
        // sound effect and etc.
    }

    private void DashBegin()
    {
        dashStartedOnGround = onGround;

        dashTimer = DashTime;
        dashCoolDownTimer = DashCoolDown;
        dashRefillCooldownTimer = DashRefillCooldown;
        
        dashTrailTimer = DashTrailTimeArray[0];
        trailsLeft = DashTrailTimeArray.Length;
        
        Level.Shake(0.3f, 1f);
        
        // set direction and speed
        DashDir = new Vector2(inputX, inputY).normalized;
        if (DashDir.magnitude < 1) 
            DashDir = Vector2.right * (facingRight ? 1 : -1);
        
        var newSpeed = DashDir;
        newSpeed.x *= DashSpeed;
        newSpeed.y *= DashSpeed * DashYRatio;
        if (Math.Sign(Speed.x) == Math.Sign(newSpeed.x) && Abs(Speed.x) > Abs(newSpeed.x))
            newSpeed.x = Speed.x;
        Speed = newSpeed;
    }

    private void DashEnd()
    {
    }

    private int DashUpdate()
    {
        if (dashTimer <= 0f) return StateNormal;
        dashTimer -= deltaTime;
        
        
        if (DashDir.y == 0)
        {
            //JumpThru Correction
            
            //Super Jump
            // if (CanUnDuck && jumpPressed && jumpGraceTimer > 0)
            // {
            //     SuperJump();
            //     return StNormal;
            // }
        }
        
        return StateDash;
    }

    // put where??
    private IEnumerator DashCoroutine()
    {
        yield return null;
        

        CallDashEvents();
            
        //Feather particles
        
        //Dash Slide
        
    }
    
    private void CreateTrail()
    {
         var trail = Instantiate(playerTrailPrefab, transform.position, quaternion.identity)
            .GetComponent<PlayerTrail>();
         
         trail.Init(facingRight, sr.sprite);
    }

    #endregion
    private void RefillDash()
    {
        Dashes = MaxDashes; // one for now.
    }
}


