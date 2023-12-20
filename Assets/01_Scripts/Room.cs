using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public HashSet<Solid> Solids;
    public HashSet<Actor> Actors;
    [HideInInspector] public Tilemap StaticTilemap;
    
    private void Awake()
    {
        // Solids, Actors, Tilemap will be set in start method of each class
        
        Solids = new HashSet<Solid>();
        Actors = new HashSet<Actor>();
        StaticTilemap = GetComponent<Tilemap>();
    }
}


