
using System;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

public class Player : Actor
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
    [SerializeField] private GameObject deathCircleObj;
    
    private SpriteRenderer sr;
    private Animator anim;

    private float gravityAccel;
    private bool onGround;
    private bool wasGround;


    
    // state machine 구현해서 death 처리하자!

    protected override void Start()
    {
        base.Start();

        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wasGround = true;
    }

    private void Update()
    {
        var inputX = Input.GetAxisRaw("Horizontal");
        var inputJump = Input.GetKeyDown(KeyCode.C);
        var deltaTime = Time.deltaTime;
        
        // spike check
        if (IsTouchingStaticTileType(TileType.Spike))
        {
            // on death
            var deathDir = new Vector2Int(160, 90) - PosCenterWS;
            Die(deathDir.normalized);
        }
        
        // ground check
        onGround = GetTileFromOS(Vector2Int.down) == TileType.Grey;

        // // landing frame
        // if (onGround && !wasGround)
        // {
        //     Speed.y = 0;
        // }
        wasGround = onGround;
        
        // move
        if (Abs(Speed.x) > maxRun)
            Speed.x = MathUtil.Appr(Speed.x, Sign(Speed.x) * maxRun, deccel);
        else
            Speed.x = MathUtil.Appr(Speed.x, inputX * maxRun, accel);
            
        // flip
        if (Speed.x != 0)
            sr.flipX = Speed.x < 0;
        
        // jump
        if (inputJump && onGround)
        {
            Speed.y = jumpStrength;
        }
        
        // gravity
        gravityAccel = gravityValue;
        
        // at the top of jump
        if (Abs(Speed.y) <= gravityIncreaseThreshold)
            gravityAccel *= 0.5f;

        if (!onGround)
        {
            Speed.y = MathUtil.Appr(Speed.y, maxFall, gravityAccel);
        }
            
        
        // get player tile position
        // then check bottom is solid

        var moveAmount = Speed *  deltaTime;
        
        MoveX(moveAmount.x, null);
        MoveY(moveAmount.y, null);

        UpdatePosition();
    }

    void Die(Vector2 direction)
    {
        // 튕기는건 나중에 구현하고 우선 쪼개지는 거부터

        for (var dir = 0; dir <= 7; dir++)
        {
            var angle = dir / 4f * PI;
            var spd = new Vector2(Cos(angle), Sin(angle)) * 5;
            // create new circles at center of player and spread out
            var animHandler= Instantiate(deathCircleObj, PosCenterWS, Quaternion.identity)
                .GetComponent<PlayerDeadBody>();
            animHandler.Play(spd);
        }
    }
}


