using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public HashSet<Solid> Solids;
    public HashSet<Actor> Actors;
    [HideInInspector] public Tilemap Tilemap;
    
    private void Awake()
    {
        Solids = new HashSet<Solid>();
        Actors = new HashSet<Actor>();
        Tilemap = RoomManager.Instance.DefaultTilemap;
    }
}


