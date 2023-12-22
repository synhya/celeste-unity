using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [FormerlySerializedAs("EnteringPower")]
    [Header("Player Settings")]
    [Tooltip("In case player is coming from down")]
    public float EnteringJumpPower = 50f;
    [FormerlySerializedAs("RespawnPos")]
    public Vector2Int SpawnPos;
    
    [Header("Link Settings")]
    // doors can be in bottom.
    [Tooltip("Offset from RoomOrigin")]
    public RectInt[] Doors;
    [Tooltip("Index should match linked doors")]
    public Room[] NextRooms;
    
    
    public HashSet<Solid> Solids;
    public HashSet<Actor> Actors;
    [HideInInspector] public Tilemap StaticTilemap;
    
    public Vector2Int OriginWS => originWs;
    private Vector2Int originWs;

    #region Validate

    private void OnValidate()
    {
        // snap respawn y to 8
        SpawnPos.y = (SpawnPos.y / 8) * 8;
    }

    #endregion
    
    private void Awake()
    {
        // Solids, Actors(except player?), Tilemap will be set in start method of each class
        
        Solids = new HashSet<Solid>();
        Actors = new HashSet<Actor>();
        
        StaticTilemap = GetComponent<Tilemap>();
        StaticTilemap.CompressBounds();
        
        // var cellSize = StaticTilemap.cellSize;
        // var bound = StaticTilemap.cellBounds;
        originWs = Vector2Int.RoundToInt(transform.position);

        for (int i = 0; i < Doors.Length; i++)
        {
            Doors[i].position += originWs;
        }
    }
}


