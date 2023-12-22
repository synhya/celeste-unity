using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager I =>
        (instance ??= (new GameObject("Manager")).AddComponent<GameManager>());
    private static GameManager instance = null;
    
    public GameObject PlayerPrefab;
    [SerializeField] private Room startingRoom;
    
    [HideInInspector] public Room CurrentRoom;
    [HideInInspector] public Player Player;

    private Camera cam;

    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
        
        CurrentRoom = startingRoom;
        cam = Camera.main;
    }

    private void Start()
    {
        SpawnPlayer();
        float x = startingRoom.OriginWS.x + 160;
        float y = startingRoom.OriginWS.y + 94;
        cam.transform.position = new Vector3(x, y, -10);
    }

    public void SwitchRoom(Room nextRoom)
    {
        CurrentRoom = nextRoom;
        Player.OnSwitchRoom(nextRoom);
    }
    
    public void SpawnPlayer()
    {
        Player = Instantiate(PlayerPrefab, (Vector3Int)CurrentRoom.SpawnPos, 
            quaternion.identity).GetComponent<Player>();
    }
} 
