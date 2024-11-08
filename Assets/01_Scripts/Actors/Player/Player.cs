﻿
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
    public const int StateNormal = 0;
    public const int StateDash = 1;
    public const int StateTransition = 2;
    public const int StateIntroJump = 3;
    public int State
    {
        get => sm.State;
        set => sm.State = value;
    }
    
    private DustVisualization Dust => EffectManager.GetDust();
    private DashLineVisualization DashLine => EffectManager.GetDashLine();
    
    // effect
    private SpriteRenderer sr;
    private HoodColorHandler hoodColorHandler;

    protected override void Awake()
    {
        base.Awake();
        
        sr = GetComponent<SpriteRenderer>();
        hoodColorHandler = GetComponent<HoodColorHandler>();
        hoodColorHandler.Mat = sr.material;
        hoodColorHandler.HasToRollBack = () => Dashes > 0;
        anim = GetComponent<Animator>();
        
        sm = new StateMachine(3);
        sm.SetCallbacks(StateNormal, NormalUpdate, NormalBegin, null);
        sm.SetCallbacks(StateDash, DashUpdate, DashBegin, DashEnd);
        sm.SetCallbacks(StateTransition, TransitionUpdate, TransitionBegin, TransitionEnd);
        sm.SetCallbacks(StateIntroJump, IntroJumpUpdate, IntroJumpBegin, IntroJumpEnd);
        sm.State = StateNormal;
    }

    protected override void Start()
    {
        base.Start();

        normalHitBoxSize = HitboxSize;
        duckHitBoxSize = normalHitBoxSize;
        duckHitBoxSize.y = DuckHitboxY;
    }

    public void OnSpawn()
    {
        Dead = false;
        Collideable = true;
        wasGround = true;
        
        facing = Facings.Right;
        sr.flipX = false;

        Speed = Vector2.zero;
        Remainder = Vector2.zero;
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
        
        if (inputX != 0 && !dashPressed && onGround)
        {
            if(!BaseSoundSource.isPlaying)
            {
                PlaySound(Sound.snowWalkSndData);
            }
        } else if (BaseSoundSource.clip == Sound.snowWalkSndData.clip)
        {
            StopSound();
        }
        
        // StateMachine.Update Speed is determined here
        sm.Update();

        if (!LockMovementBySpeed)
        {
            var moveAmount = Speed * Time.deltaTime;
        
            MoveH(moveAmount.x, null);
            MoveV(moveAmount.y, null);
        }

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
    
    public void OnAddStrawberry(int id)
    {
        // other things to do.
        
        SaveData.Instance.Strawberries.Add(id);
    }
}


