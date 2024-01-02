﻿using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/// <summary>
/// entire stage split to smaller levels(rooms)
/// </summary>
public class Room : MonoBehaviour
{

    [Header("Player Settings")]
    [Tooltip("In case player is coming from down")]
    public float EnteringJumpPower = 50f;
    
    public Vector2Int SpawnPos;
    [Tooltip("In case room is too large")]
    public bool CamHasToFollowPlayer = false;
    
    [Header("Link Settings")]
    // doors can be in bottom.
    [Tooltip("Offset from RoomOrigin")]
    public RectInt[] Doors;
    [Tooltip("Index should match linked doors")]
    public Room[] NextRooms;
    
    public HashSet<Solid> Solids;
    public HashSet<Trigger> Triggers;
    
    public Vector2Int OriginWS => originWs;
    private Vector2Int originWs;
    
    public RectInt Bound;
    
    
    private void Awake()
    {
        // Solids, Actors(except player?), Tilemap will be set in start method of each class
        
        Solids = new HashSet<Solid>();
        Triggers = new HashSet<Trigger>();
        
        // var cellSize = StaticTilemap.cellSize;
        // var bound = StaticTilemap.cellBounds;
        originWs = Vector2Int.RoundToInt(transform.position);

        for (int i = 0; i < Doors.Length; i++)
        {
            Doors[i].position += originWs;
        }
    }
}


