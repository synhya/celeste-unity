using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{
    public Player Player;
    [SerializeField] private Room startingRoom;
    
    /// <summary>
    /// set on awake
    /// </summary>
    [HideInInspector] public Room CurrentRoom;
    [HideInInspector] public Tilemap Map;

    public bool InSpace = false;
    private const float RoomSwitchTime = 0.8f;
    public HashSet<Actor> AllActors;

    void Awake() 
    {
        CurrentRoom = startingRoom;
        Map = GetComponent<Tilemap>();
        Map.CompressBounds();
        AllActors = new HashSet<Actor>();

        foreach (Transform room in transform)
        {
            if(room.gameObject.name.Contains("Room"))
                room.gameObject.SetActive(false);
        }
    }

    public void StartLevel()
    {
        Player.transform.SetParent(transform);
        SpawnPlayer();
        
        startingRoom.gameObject.SetActive(true);
        float x = startingRoom.OriginWS.x + 160;
        float y = startingRoom.OriginWS.y + 94;
        EffectManager.MainCam.transform.position = new Vector3(x, y, -10f);
    }

    public void SwitchRoom(Room nextRoom)
    {
        CurrentRoom.gameObject.SetActive(false);
        nextRoom.gameObject.SetActive(true);
        
        CurrentRoom = nextRoom;
        Player.Added(nextRoom);
        
        FreezeLevel(RoomSwitchTime);
    }
    
    public void SpawnPlayer()
    {
        Player.gameObject.SetActive(true);
        Player.OnSpawn();
    }
    
    public void FreezeLevel(float time)
    {
        Game.Pause();
        
        // adjust camera
        float x = CurrentRoom.OriginWS.x + 160;
        float y = CurrentRoom.OriginWS.y + 94;
        
        EffectManager.MoveCam(new Vector2(x, y), time).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            Game.Resume();
            Player.OnSwitchRoomEnd();
        });
    }
} 
