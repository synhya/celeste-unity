
using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Mathf;

public partial class Player
{
    
    private const float DashSpeed = 240f;
    private const float EndDashSpeed = 160f; // end speed?
    private const float EndDashUpMult = .75f;
    private const float DashTime = .15f;
    private const float DashCoolDown = .2f;
    private const float DashRefillCooldown = .1f;
    private const int DashHJumpThruNudge = 6; // ?
    private const int DashCornerCorrection = 4;
    private const int DashVFloorSnapDist = 3; // vertical snap
    private const float DashAttackTime = .3f; // Attack? => brake hidden wall
    

    // vars    
    private bool dashPressed;
    
    [HideInInspector] public int Dashes;
    [HideInInspector] public Vector2? OverrideDashDirection;
    
    private float dashCoolDownTimer;
    private float dashRefillCooldownTimer;
    [HideInInspector] public Vector2 DashDir;

    private float dashAttackTimer;
    private bool dashStartedOnGround;

    private bool calledDashEvents;
    private int lastDashes;

    private float dashTrailTimer;

    // use shader?
    public static readonly Color NormalHairColor = new Color(0.67f, 0.2f, 0.2f);
    public static readonly Color UsedHairColor = new Color(0.27f, 0.72f, 1f);
    public static readonly Color FlashHairColor = Color.white;

    private float hairFlashTimer;
    public Color? OverrideHairColor;
    
    private Vector2 beforeDashSpeed;
    private bool wasDashB;
    private bool launched;

    /// <summary>
    /// Platform lift value ( leave 0 for now )
    /// </summary>
    private Vector2 LiftBoost
    {
        get
        {
            // Vector2 val = LiftSpeed;
            //
            // if (Math.Abs(val.X) > LiftXCap)
            //     val.X = LiftXCap * Math.Sign(val.X);
            //
            // if (val.Y > 0)
            //     val.Y = 0;
            // else if (val.Y < LiftYCap)
            //     val.Y = LiftYCap;
            //
            // return val;
            return Vector2.zero;
        }
    }

    public int StartDash()
    {
        wasDashB = Dashes == 2;
        Dashes = Max(0, Dashes - 1);
        return StDash;
    }

    public bool DashAttacking
    {
        get {
            return dashAttackTimer > 0;
        }
    }
    
    public bool CanDash
    {
        get
        {
            return dashPressed && dashCoolDownTimer <= 0 && Dashes > 0;
        }
    }

    #region StateBehavior

    private void CallDashEvents() { }

    private void DashBegin()
    {
        calledDashEvents = false;
        dashStartedOnGround = onGround;
        launched = false;

        dashCoolDownTimer = DashCoolDown;
        dashRefillCooldownTimer = DashRefillCooldown;
        dashTrailTimer = 0;
        
        Level.Shake(0.3f, 1f);

        dashAttackTimer = DashAttackTime;
        beforeDashSpeed = Speed;
        Speed = Vector2.zero;
        DashDir = Vector2.zero;

        // if (!onGround && Ducking && CanUnDuck)
        //     Ducking = false;
    }

    private void DashEnd()
    {
        CallDashEvents();
    }

    private int DashUpdate()
    {
        //Trail
        if (dashTrailTimer > 0)
        {
            dashTrailTimer -= Time.deltaTime;
            if (dashTrailTimer <= 0)
                CreateTrail();
        }
        
        //Grab Holdables
        
        
        //
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
        
        return StDash;
    }

    private IEnumerator DashCoroutine()
    {
        yield return null;

        var dir = new Vector2(Sign(Speed.x), inputY).normalized;
        // if (OverrideDashDirection.HasValue)
        //     dir = OverrideDashDirection.Value;

        var newSpeed = Speed * dir;
        if (Math.Sign(beforeDashSpeed.x) == Math.Sign(newSpeed.x) && Abs(beforeDashSpeed.x) > Abs(newSpeed.x))
            newSpeed.x = beforeDashSpeed.x;
        Speed = newSpeed;

        // if (CollideCheck<Water>())
        //     Speed *= SwimDashSpeedMult;

        DashDir = dir;

        CallDashEvents();
            
        //Feather particles
        
        //Dash Slide
        
    }
    
    private void CreateTrail()
    {
        // TrailManager.Add(this, wasDashB ? NormalHairColor : UsedHairColor);
    }

    #endregion
}


