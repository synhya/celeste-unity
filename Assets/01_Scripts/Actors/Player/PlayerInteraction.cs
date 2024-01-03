
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

// funcs in this part is called every frame
public partial class Player
{
    [Header("Interaction Settings")]
    [SerializeField] private float boostPower = 300f;

    // vars
    private int inputX;
    private int inputY;
    private bool jumpPressed;
    private bool jumpPressing;
    private float deltaTime;

    public bool Dead;


    private void CheckInput()
    {
        deltaTime = Time.deltaTime;

        if (forceMoveXTimer > 0)
        {
            forceMoveXTimer -= deltaTime;
            inputX = forceMoveX;
        }
        else
        {
            inputX = (int)Input.GetAxisRaw("Horizontal");
        }
        
        
        inputY = (int)Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetKeyDown(KeyCode.C);
        if (jumpPressed)
        {
            jumpBufferTimer = JumpBufferTime;
        }
        
        dashPressed = Input.GetKeyDown(KeyCode.X);
        jumpPressing = Input.GetKey(KeyCode.C);
    }
    
    private void CheckOverlaps()
    {
        // room bound check is done in level
        
        // spike check
        if (OverlapTileFlagCheckOS(TileType.Obstacle, Vector2Int.zero, out var obsType))
        {
            var deathDir = Vector2.zero;
            
            if (obsType.HasFlag(TileType.Spike))
                deathDir = -Speed;
            
            Die(deathDir.normalized);
        }
        
        // ground check
        onGround = CollideCheck(Vector2Int.down);
        
        // landing check
        isLanding = onGround && !wasGround;
        isTakingOff = wasGround && !onGround;

        // trigger check 
        foreach (Trigger trigger in Room.Triggers)// change to Tracker.GetEntities<Trigger>()
        {
            if (CollideCheck(trigger))
            {
                if (!trigger.Triggered)
                {
                    trigger.Triggered = true;
                    trigger.OnEnter(this);
                }
                trigger.OnStay(this);
            }
            else if (trigger.Triggered)
            {
                trigger.Triggered = false;
                trigger.OnLeave(this);
            }
        }
        
        // ice check .. etc
    }

    public void OnBoost()
    {
        RefillDash();
        Speed.y = boostPower;
    }
    
    public void Die(Vector2 knockBackDir)   
    {
        if (!Dead)
        {
            Dead = true;
            Collideable = false;
            trailsLeft = 0;
        
            // camera shake
            EffectManager.ShakeCam(0.5f, 1.5f);
        
            // spawn dead body (dont set as parent as it will be disabled)
            var body = Instantiate(deadBodyPrefab, CenterWS, Quaternion.identity)
                .GetComponent<PlayerDeadBody>();
            body.Init(knockBackDir, SR.flipX ^ flipAnimFlag);
            // body.DeathAction = () => {}
        
            // change stats ( Stats.Death++; .. }
            
            gameObject.SetActive(false);
        }
    }
}


