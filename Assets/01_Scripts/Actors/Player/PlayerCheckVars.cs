
using UnityEngine;
using static UnityEngine.Mathf;

// funcs in this part is called every frame
public partial class Player
{
    [Header("Interaction Settings")]
    [SerializeField] private float SpringYPower = 220f;
    

    // vars
    private int inputX;
    private int inputY;
    private bool jumpPressed;
    private bool jumpPressing;
    private float deltaTime;


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
        var hitbox = HitBoxWS;
        
        // doorCheck  precedes fall
        var doors = Room.Doors;
        for (int i = 0; i < doors.Length; i++)
        {
            if (hitbox.Overlaps(doors[i]))
                level.SwitchRoom(Room.NextRooms[i]);
        }

        if (invinsibleTimer > 0f) invinsibleTimer -= deltaTime;
        else
        {
            // fall death check 320 * 180
            if (PositionWS.y < Room.OriginWS.y && Speed.y < 0f)
            {
                // should also check if there is another room!
            
                Die(Vector2.up);
            }

            // spike check
            if (OverlapTileFlagCheckOS(TileType.Obstacle,Vector2.zero, out var obsType))
            {
                var deathDir = Vector2.zero;
            
                if (obsType == TileType.Spike)
                    deathDir = -Speed;
            
                Die(deathDir.normalized);
            }
        }
        
        // ground check
        onGround = (OverlapTileFlagCheckOS(TileType.Ground, Vector2.down, 0, -1) &&
                    !OverlapTileFlagCheckOS(TileType.HalfGround, Vector2.zero));
        
        // landing check
        isLanding = onGround && !wasGround;
        isTakingOff = wasGround && !onGround;

        // spring check (spring is not tile)
        if (!onGround)
        {
            int offsetX = Speed.x != 0f ? (int)Sign(Speed.x) : 0;
            int offsetY = Speed.y >= 0f ? 0 : -1;
            
            if (CollideCheck<Spring>(offsetX, offsetY))
            {
                RefillDash();
                Speed.y = SpringYPower;
            }
        }
        
        // ice check .. etc
    }
    
    void Die(Vector2 knockBackDir)
    {
        // camera shake
        level.Shake(0.5f, 1.5f);
        
        // spawn dead body (dont set as parent as it will be disabled)
        var body = Instantiate(deadBodyPrefab, transform.position, Quaternion.identity)
            .GetComponent<PlayerDeadBody>();
        body.Init(knockBackDir, sr.flipX ^ flipAnimFlag);
        // body.DeathAction = () => {}
        
        // change stats ( Stats.Death++; .. }
        
        
        Destroy(gameObject);
    }
}


