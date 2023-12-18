using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomMananger : MonoBehaviour
{
    public static RoomMananger Room =>
        (instance ??= (new GameObject("Manager")).AddComponent<RoomMananger>());
    private static RoomMananger instance = null;


    public List<Solid> Solids;
    public List<Actor> Actors;
    public Tilemap Tilemap;
    
    private void Awake()
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);

        Solids = new List<Solid>();
    }
}


