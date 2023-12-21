using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager I =>
        (instance ??= (new GameObject("Manager")).AddComponent<GameManager>());
    private static GameManager instance = null;
    
    public Room CurrentRoom;
    public GameObject PlayerPrefab;

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
    public void RespawnPlayer()
    {
        Instantiate(PlayerPrefab, CurrentRoom.RespawnPos, quaternion.identity);
    }
} 
