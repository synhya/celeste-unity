using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public Vector2Int RoomCoordinate = Vector2Int.zero;
    
    public HashSet<Solid> Solids;
    public HashSet<Actor> Actors;
    [HideInInspector] public Tilemap StaticTilemap;

    // create editor script
    public Vector3Int RespawnPos;
    
    private void Awake()
    {
        // Solids, Actors, Tilemap will be set in start method of each class
        
        Solids = new HashSet<Solid>();
        Actors = new HashSet<Actor>();
        StaticTilemap = GetComponent<Tilemap>();
    }

    private void Start()
    {
        
    }
}


