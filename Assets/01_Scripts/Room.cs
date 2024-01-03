 using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;


public enum DoorDirections
{
    Up,
    Down,
    Right,
    Left,
}

[Serializable]
public struct Door
{
    public int StartPosWS;
    public int Length;
    public DoorDirections Dir;
    public Vector2Int TransPosWS;
}

[Serializable]
public struct RoomLink
{
    public Door Door;
    public Room Room;
}

/// <summary>
/// entire stage split to smaller levels(rooms)
/// </summary>
public class Room : MonoBehaviour
{
    public Vector2Int SpawnPosWS;
    [FormerlySerializedAs("TransitionTargetPos")]
    
    public RoomLink[] RoomLinks;
    
    public HashSet<Solid> Solids;
    public HashSet<Trigger> Triggers;
    
    [FormerlySerializedAs("BoundRectWS")]
    public RectInt BoundRect
        = new RectInt(Vector2Int.zero, new Vector2Int(Game.Width, Game.Height));
    
    private void Awake()
    {
        Solids = new HashSet<Solid>();
        Triggers = new HashSet<Trigger>();

        var bottomLeft = Vector2Int.RoundToInt(transform.position) - BoundRect.size / 2;

        // move to world space
        for (int i = 0; i < RoomLinks.Length; i++)
        {
            if(RoomLinks[i].Door.Dir == DoorDirections.Up ||
               RoomLinks[i].Door.Dir == DoorDirections.Down)
                RoomLinks[i].Door.StartPosWS += bottomLeft.x;
            else 
                RoomLinks[i].Door.StartPosWS += bottomLeft.y;
            
            RoomLinks[i].Door.TransPosWS += bottomLeft;
        }
        SpawnPosWS += bottomLeft;
        BoundRect.position = bottomLeft;
    }
}


