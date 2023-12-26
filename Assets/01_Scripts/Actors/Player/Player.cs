
using System;
using System.Linq;
using System.Net.Http.Headers;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;
enum Facing
{
    Right = 1,
    Left = -1,
}

public partial class Player : Actor
{
    private Level level;
    
    [Header("OtherObjs")]
    [SerializeField] private GameObject deadBodyPrefab;

    private const float InvinsibleTimeOnSwitch = 0.5f;
    private float invinsibleTimer;

    private float gravityAccel;
    
    // state vars
    private bool onGround;
    private bool wasGround;
    private bool isLanding;
    private bool isTakingOff;
    private int wallSlideDir;
    
    private Facing facing;
    
    [HideInInspector] public bool IsPaused;

    // State Machine
    private StateMachine sm;
    private DustVisualization dust;
    
    public const int StateNormal = 0;
    public const int StateDash = 1;
    
    protected override void FindRoom()
    {
        Room = Game.G.CurrentLevel.CurrentRoom;
        level = Room.Level;
    }

    protected override void Start()
    {
        base.Start();

        InitAnimation();
        
        sm = new StateMachine(3);
        sm.SetCallbacks(StateNormal, NormalUpdate, NormalBegin, null);
        sm.SetCallbacks(StateDash, DashUpdate, DashBegin, DashEnd);
        sm.State = StateNormal;
        
        dust = EffectManager.Instance.Dust;
        
        wasGround = true;
        facing = Facing.Right;
    }

    private void Update()
    {
        if(IsPaused) return;
        
        CheckInput();
        CheckOverlaps();
        
        //Vars
        {
            // landing and taking-off
            if(isLanding) dust.Burst(PositionWS + Vector2.up, new Vector2(5, 3) ,Vector2.up, 1.5f);

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
        
        // should be called after speed is confirmed
        SetAnimation();

        var moveAmount = Speed *  deltaTime;
        
        MoveX(moveAmount.x, null);
        MoveY(moveAmount.y, null);

        UpdatePosition();

        SetPreviousValues();
    }

    // call at the end of update or lateUpdate
    private void SetPreviousValues()
    {
        wasGround = onGround;
    }

    public override void Squish()
    {
        Die(Vector2.up);
    }
    

    public void OnSwitchRoomStart(Room nextRoom)
    {
        
        
        // if going up -> speedup
        if (Speed.y > 0)
            Speed.y += nextRoom.EnteringJumpPower;
        RefillDash();

        Room.OnActorExit(this);
        Room = nextRoom;
        Room.OnActorEnter(this);
    }
    public void OnSwitchRoomEnd()
    {
        IsPaused = false;
        invinsibleTimer = InvinsibleTimeOnSwitch;
    }
}


