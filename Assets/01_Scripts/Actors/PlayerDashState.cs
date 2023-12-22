
using UnityEngine;

public partial class Player
{
    // public const int StNormal = 0;
    // public const int StClimb = 1;
    // public const int StDash = 2;

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

    public static readonly Color NormalHairColor = new Color(0.67f, 0.2f, 0.2f);
    public static readonly Color UsedHairColor = new Color(0.27f, 0.72f, 1f);
    public static readonly Color FlashHairColor = Color.white;
    
    
}


