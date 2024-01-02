
using System;
using System.Linq;
using System.Net.Http.Headers;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

public enum Facings
{
    Right = 1,
    Left = -1,
}

public partial class Player : Actor
{
    [Header("OtherObjs")]
    [SerializeField] private GameObject deadBodyPrefab;

    private float gravityAccel;
    
    // state vars
    private bool onGround;
    private bool wasGround;
    private bool isLanding;
    private bool isTakingOff;
    private int wallSlideDir;
    private bool wasDucking;
    
    private Facings facing;

    // State Machine
    private StateMachine sm;
    private DustVisualization Dust => EffectManager.GetDust();
    private DashLineVisualization DashLine => EffectManager.GetDashLine();
    
    public const int StateNormal = 0;
    public const int StateDash = 1;
    public const int StateIntroRespawn = 2;
    public const int StateIntroWalk = 3;
    public const int StateIntroJump = 4;
    public const int StateReflectionFall = 5; // ?
    
    // effect
    [HideInInspector] public SpriteRenderer SR;

    protected override void Start()
    {
        base.Start();

        SR = GetComponent<SpriteRenderer>();
        InitAnimation();
        
        sm = new StateMachine(3);
        sm.SetCallbacks(StateNormal, NormalUpdate, NormalBegin, null);
        sm.SetCallbacks(StateDash, DashUpdate, DashBegin, DashEnd);
        sm.State = StateNormal;

        normalHitBoxSize = HitboxSize;
        duckHitBoxSize = normalHitBoxSize;
        duckHitBoxSize.y = DuckHitboxY;
    }

    public void OnSpawn()
    {
        Collideable = true;
        wasGround = true;
        
        facing = Facings.Right;
        SR.flipX = false;

        Speed = Vector2.zero;
        Remainder = Vector2.zero;
        PositionWS = Room.SpawnPos;
    }


    protected override void Update()
    {
        if(Game.IsPaused) return;
        
        CheckInput();
        CheckOverlaps();
        
        //Vars
        {
            // landing and taking-off
            if(isLanding) Dust.Burst(PositionWS + Vector2.up, new Vector2(5, 3) ,Vector2.up, 1.5f);

            if (jumpBufferTimer > 0f) jumpBufferTimer -= deltaTime;
        
            //Wall Slide
            if (wallSlideDir != 0)
            {
                wallSlideTimer = Math.Max(wallSlideTimer - deltaTime, 0);
                wallSlideDir = 0;
            }
            if(onGround) 
                wallSlideTimer = WallSlideTime;
            if (wallSlideParticleTimer > 0f) wallSlideParticleTimer -= deltaTime;
            
            //Jump Grace
            if (onGround) jumpGraceTimer = JumpGraceTime;
            else if (jumpGraceTimer > 0f) jumpGraceTimer -= deltaTime;
            
            // Dashes
            {
                if (dashCoolDownTimer > 0)
                    dashCoolDownTimer -= deltaTime;
                
                if (dashRefillCooldownTimer > 0)
                    dashRefillCooldownTimer -= deltaTime;
                else if (onGround)
                    RefillDash();

                if (trailsLeft > 0)
                {
                    if (dashTrailTimer <= 0)
                    {
                        CreateTrail();
                        trailsLeft -= 1;
                        dashTrailTimer = trailsLeft > 0 ? DashTrailTimeArray[^trailsLeft] : 0;
                    } else 
                        dashTrailTimer -= deltaTime;
                }
            }
            
            //Var Jump
            if (varJumpTimer > 0f) varJumpTimer -= deltaTime;
        }
        
        
        // StateMachine.Update
        sm.Update();
        
        base.Update();
        
        var moveAmount = Speed * Time.deltaTime;
        
        MoveH(moveAmount.x, null);
        MoveV(moveAmount.y, null);

        UpdatePosition();
        
        // should be called after speed is confirmed
        SetAnimation();

        SetPreviousValues();
    }

    // call at the end of update or lateUpdate
    private void SetPreviousValues()
    {
        wasGround = onGround;
        wasDucking = Ducking;
    }

    public override void Squish(CollisionData data)
    {
        Die(Vector2.up);
    }

    public void OnResume()
    {

    }
    public void OnAddStrawberry(int id)
    {
        // other things to do.
        
        SaveData.Instance.Strawberries.Add(id);
    }
}


