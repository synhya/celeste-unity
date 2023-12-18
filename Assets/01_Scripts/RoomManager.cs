using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance =>
        (instance ??= (new GameObject("Manager")).AddComponent<RoomManager>());
    private static RoomManager instance = null;
    
    public Tilemap DefaultTilemap;
    public Room CurrentRoom;

    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
    }

    void OnRoomChange(Room nextRoom)
    {
        CurrentRoom = nextRoom;
    }
} 
