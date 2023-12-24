
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

public partial class Player
{
    [Header("Interaction Settings")]
    [SerializeField] private float SpringYPower = 220f;
    
    [Header("Input Settings")]
    [SerializeField] private float ShortJumpThreshold = 0.1f;
    [SerializeField] private float JumpBufferTime = 0.1f;
    
    // vars
    private float inputX;
    private float inputY;
    private bool shortJumpPressed;
    private bool longJumpPressed;
    private float jumpPressTimer;
    private float deltaTime;
    
    private bool checkJumpPressTime;
    private float jumpBufferTimer;

    private void CheckInput()
    {
        deltaTime = Time.deltaTime;
        
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        
        // check jump
        if (Input.GetKeyDown(KeyCode.C))
        {
            checkJumpPressTime = true;
            jumpPressTimer = ShortJumpThreshold;
        }
        if (checkJumpPressTime)
        {
            if (jumpPressTimer > 0f)
            {
                jumpPressTimer -= deltaTime;
                if (Input.GetKeyUp(KeyCode.C))
                {
                    shortJumpPressed = true;
                    checkJumpPressTime = false;
                    jumpBufferTimer = JumpBufferTime;
                }
            }
            else if (Input.GetKey(KeyCode.C))
            {
                longJumpPressed = true;
                checkJumpPressTime = false;
                jumpBufferTimer = JumpBufferTime;
            }
        }
        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= deltaTime;

        dashPressed = Input.GetKeyDown(KeyCode.X);
    }
    
    private void CheckOverlaps()
    {
        var hitbox = HitBoxWS;
        
        // doorCheck  precedes fall
        var doors = Room.Doors;
        for (int i = 0; i < doors.Length; i++)
        {
            if (hitbox.Overlaps(doors[i]))
                Level.SwitchRoom(Room.NextRooms[i]);
        }
        
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
        
        // ground check
        onGround = (OverlapTileFlagCheckOS(TileType.Ground, Vector2.down, 0, -1) &&
                    !OverlapTileFlagCheckOS(TileType.HalfGround, Vector2.zero));
        
        // landing check
        isLanding = onGround && !wasGround;
        isTakingOff = wasGround && !onGround;

        wallDir = 0;
        // wall check
        if (!onGround)
        {
            if (OverlapTileFlagCheckOS(TileType.Ground, Vector2.right, 1, 0))
                wallDir = 1;
            else if (OverlapTileFlagCheckOS(TileType.Ground, Vector2.left, -1, 0))
                wallDir = -1;
        }

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
}


