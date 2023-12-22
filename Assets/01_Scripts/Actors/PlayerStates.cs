
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

public partial class Player
{
    private void FigureOutCurrentState()
    {
        var hitbox = HitBoxWS;
        
        // doorCheck  precedes fall
        var doors = Room.Doors;
        for (int i = 0; i < doors.Length; i++)
        {
            if (hitbox.Overlaps(doors[i]))
                GameManager.I.SwitchRoom(Room.NextRooms[i]);
        }
        
        // fall death check 320 * 180
        if (PositionWS.y < Room.OriginWS.y && Speed.y < 0f)
        {
            // should also check if there is another room!
            
            Die(Vector2.up);
        }

        // spike check
        if (IsTouchingStaticTileType(TileType.Spike))
        {
            var deathDir = -Speed;
            Die(deathDir.normalized);
        }
        
        // ground check
        onGround = false;
        for (int i = hitbox.xMin + 1; i < hitbox.xMax; i++)
        {
            int j = hitbox.yMin - 1;

            if (GetTileFromWS(new Vector2Int(i, j)) == TileType.Grey)
            {
                onGround = true;
                break;
            }
        }
        
        // spring check (spring is not tile)
        if (!onGround)
        {
            foreach (var solid in Room.Solids)
            {
                if (solid is Spring)
                {
                    var spr = solid as Spring;
                    int offsetX = Speed.x != 0f ? (int)Sign(Speed.x) : 0;
                    int offsetY = Speed.y >= 0f ? 0 : -1;
                    if (IsEntityTypeAt(offsetX, offsetY, spr))
                    {
                        spr.ActivateSpring();
                        Speed.y = spr.Strength;
                    }
                }
            }
        }
        
        // ice check .. etc
    }
}


