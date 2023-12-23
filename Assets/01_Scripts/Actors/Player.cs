
using System;
using System.Net.Http.Headers;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

public partial class Player : Actor
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
    
    [Header("Prefabs")]
    [SerializeField] private GameObject deadBodyPrefab;
    
    private SpriteRenderer sr;

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

    // 늘어날 수록 복잡해진다. state machine쓰자
    public const int StNormal = 0;
    public const int StClimb = 1;
    public const int StDash = 2;
    
    protected override void FindRoom()
    {
        Room = Game.G.CurrentLevel.CurrentRoom;
    }

    protected override void Start()
    {
        base.Start();

        InitAnimation();
        
        sr = GetComponent<SpriteRenderer>();
        
        wasGround = true;
    }

    private void Update()
    {
        if(IsPaused) return;
        
        InputCheck();

        UpdateBools();
        
        // move
        if (Abs(Speed.x) > maxRun)
            Speed.x = MathUtil.Appr(Speed.x, Sign(Speed.x) * maxRun, deccel);
        else
            Speed.x = MathUtil.Appr(Speed.x, inputX * maxRun, accel);
        
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


    private void InputCheck()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetKeyDown(KeyCode.C);
        dashPressed = Input.GetKeyDown(KeyCode.X);
        deltaTime = Time.deltaTime;
        
        // landing frame
        if (onGround && !wasGround)
        {
            
        }
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

    public void OnSwitchRoom(Room nextRoom)
    {
        // if going up -> speedup
        if (Speed.y > 0)
            Speed.y += nextRoom.EnteringJumpPower;

        Room.OnActorExit(this);
        Room = nextRoom;
        Room.OnActorEnter(this);
    }
}


