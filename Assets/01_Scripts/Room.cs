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
    public int StartPos;
    public int Length;
    public DoorDirections Dir;
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
    public Vector2Int SpawnPos;
    [FormerlySerializedAs("TransitionTargetPos")]
    public Vector2Int TransitionPos;
    
    public RoomLink[] RoomLinks;
    
    public HashSet<Solid> Solids;
    public HashSet<Trigger> Triggers;
    
    [FormerlySerializedAs("BoundRectWS")]
    public RectInt BoundRect
        = new RectInt(Vector2Int.zero, new Vector2Int(CamWidth, CamHeight));
    
    public const int CamWidth = 320;
    public const int CamHeight = 184;
    
    private void Awake()
    {
        Solids = new HashSet<Solid>();
        Triggers = new HashSet<Trigger>();

        var pos = Vector2Int.RoundToInt(transform.position);

        BoundRect.position = pos - BoundRect.size / 2;
    }
}


