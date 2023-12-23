
using System;
using System.Linq;
using System.Net.Http.Headers;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

public partial class Player : Actor
{
    [Header("Prefabs")]
    [SerializeField] private GameObject deadBodyPrefab;

    private float inputX;
    private float inputY;
    private bool jumpPressed;
    private float deltaTime;

    private float gravityAccel;
    
    // state vars
    private bool onGround;
    private bool wasGround;
    private bool isLanding;
    
    private bool jumpConfirmed;
    
    [HideInInspector] public bool IsPaused;

    // State Machine
    private StateMachine sm;
    
    public const int StateNormal = 0;
    public const int StateClimb = 1;
    public const int StateDash = 2;
    
    protected override void FindRoom()
    {
        Room = Game.G.CurrentLevel.CurrentRoom;
    }

    protected override void Start()
    {
        base.Start();

        InitAnimation();
        
        sm = new StateMachine(3);
        sm.SetCallbacks(StateNormal, NormalUpdate, null, null);
        sm.SetCallbacks(StateDash, DashUpdate, DashBegin, DashEnd);
        sm.State = StateNormal;
        
        wasGround = true;
    }

    private void Update()
    {
        if(IsPaused) return;
        
        CheckInput();
        CheckOverlaps();
        
        // Update for dashes
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

    void Die(Vector2 knockBackDir)
    {
        // camera shake
        Level.Shake(0.5f, 1.5f);
        
        // spawn dead body (dont set as parent as it will be disabled)
        var body = Instantiate(deadBodyPrefab, transform.position, quaternion.identity)
            .GetComponent<PlayerDeadBody>();
        body.Init(knockBackDir, sr.flipX ^ flipAnimFlag);
        // body.DeathAction = () => {}
        
        // change stats ( Stats.Death++; .. }
        
        
        Destroy(gameObject);
    }

    public override void Squish()
    {
        Die(Vector2.up);
    }

    public void OnSwitchRoom(Room nextRoom)
    {
        // if going up -> speedup
        if (Speed.y > 0)
            Speed.y += nextRoom.EnteringJumpPower;
        RefillDash();

        Room.OnActorExit(this);
        Room = nextRoom;
        Room.OnActorEnter(this);
    }
}


