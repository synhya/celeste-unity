
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
        wallSlideTimer = WallSlideTime;
        
        dashStartedOnGround = onGround;

        dashTimer = DashTime;
        dashCoolDownTimer = DashCoolDown;
        dashRefillCooldownTimer = DashRefillCooldown;
        
        dashTrailTimer = DashTrailTimeArray[0];
        trailsLeft = DashTrailTimeArray.Length;
        
        // set direction and speed
        DashDir = new Vector2(inputX, inputY).normalized;
        if (DashDir.magnitude < 1) 
            DashDir = Vector2.right * (int)facing;
        
        var newSpeed = DashDir;
        newSpeed.x *= DashSpeed;
        newSpeed.y *= DashSpeed * DashYRatio;
        if (Math.Sign(Speed.x) == Math.Sign(newSpeed.x) && Abs(Speed.x) > Abs(newSpeed.x))
            newSpeed.x = Speed.x;
        Speed = newSpeed;
        
        // never duck when dashing
        if (CanUnDuck)
            Ducking = false;
        
        // effects
        DashLine.Cast(CenterWS, DashDir);
        // EffectManager.ShakeCam(0.3f, 1.4f);
        EffectManager.ChangeCloth();
        EffectManager.CreateRipple(CenterWS);
        EffectManager.ShakeCam(0.3f, 0.8f);
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
        // get trail from effect manager.
        var trail = EffectManager.GetTrail();
        trail.Play(transform.position, facing == Facing.Right, SR.sprite);
    }

    #endregion
    public void RefillDash()
    {
        Dashes = MaxDashes; // one for now.
    }
}


