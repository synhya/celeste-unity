
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

public partial class Player
{
    [Header("Interaction Settings")]
    [SerializeField] private float SpringYPower = 220f;
    
    private void CheckInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetKeyDown(KeyCode.C);
        dashPressed = Input.GetKeyDown(KeyCode.X);
        deltaTime = Time.deltaTime;
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


