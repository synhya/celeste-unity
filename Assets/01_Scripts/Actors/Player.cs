
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
    private bool inputJump;
    private float deltaTime;

    private float gravityAccel;
    
    // state vars
    private bool onGround;
    private bool wasGround;
    
    private bool isLanding;
    
    private bool jumpConfirmed;

    // state machine 구현해서 death 처리하자!

    protected override void Start()
    {
        base.Start();

        InitAnimation();
        
        sr = GetComponent<SpriteRenderer>();
        wasGround = true;
    }

    private void Update()
    {
        InputAndStateCheck();
        
        // move
        if (Abs(Speed.x) > maxRun)
            Speed.x = MathUtil.Appr(Speed.x, Sign(Speed.x) * maxRun, deccel);
        else
            Speed.x = MathUtil.Appr(Speed.x, inputX * maxRun, accel);
        
        // confirm jump
        if (inputJump && onGround)
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


    private void InputAndStateCheck()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputJump = Input.GetKeyDown(KeyCode.C);
        deltaTime = Time.deltaTime;
        

        // spike check
        if (IsTouchingStaticTileType(TileType.Spike))
        {
            // on death
            var deathDir = -Speed;
            Die(deathDir.normalized);
        }
        
        var hitbox = HitBoxWS;
        onGround = false;
        for (int i = hitbox.xMin; i <= hitbox.xMax; i++)
        {
            int j = hitbox.yMin - 1;

            if (GetTileFromWS(new Vector2Int(i, j)) == TileType.Grey)
            {
                onGround = true;
                break;
            }
        }
        
        // ice check .. etc
        
        // landing frame
        if (onGround && !wasGround)
        {
            
        }
    }

    void Die(Vector2 knockBackDir)
    {
        // camera shake (cinemachine)
        
        // spawn dead body (dont set as parent as it will be disabled)
        var body = Instantiate(deadBodyPrefab, transform.position, quaternion.identity)
            .GetComponent<PlayerDeadBody>();
        body.Init(knockBackDir, sr.flipX ^ flipAnimFlag);
        // body.DeathAction = () => {}
        
        // change stats ( Stats.Death++; .. }
        
        
        Destroy(gameObject);
    }
}


