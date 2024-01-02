using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public enum IntroTypes
{
    Transition,
    Jump,
    Fall,
    WalkInRight,
    WalkInLeft,
    Respawn,
    None
}

public class Level : MonoBehaviour
{
    [SerializeField] private Room startingRoom;
    
    [HideInInspector] public Room CurrentRoom;
    [HideInInspector] public Tilemap Map;
    
    private float roomTransitionTime = 0.8f;
    public HashSet<Actor> AllActors;
    private Player player;
    
    public IntroTypes IntroType = IntroTypes.Jump;

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

    void Start()
    {
        player = Game.MainPlayer;
    }

    public void StartLevel()
    {
        player.transform.SetParent(transform);
        SpawnPlayer();
        
        startingRoom.gameObject.SetActive(true);

        var camPos = FindRoomStartCamPos(startingRoom);
        Game.MainCam.transform.position = new Vector3(camPos.x, camPos.y, -10f);
    }

    private void Update()
    {
        if(Game.IsPaused) return;
        
        // if (CurrentRoom.BoundRect.size.y > Room.CamHeight)
        // {
        //     var cam = Game.MainCam;
        //     var bound = new Rect();
        //     // if player out of bound follow player .
        // }
    }
    
    // // Camera (lerp by distance using delta-time)
    // if (InControl || ForceCameraUpdate)
    // {
    //     if (StateMachine.State == StReflectionFall)
    //     {
    //         level.Camera.Position = CameraTarget;
    //     }
    //     else
    //     {
    //         var from = level.Camera.Position;
    //         var target = CameraTarget;
    //         var multiplier = StateMachine.State == StTempleFall ? 8 : 1f;
    //                 
    //         level.Camera.Position = from + (target - from) * (1f - (float)Math.Pow(0.01f / multiplier, Engine.DeltaTime));
    //     }
    // }

    public void SwitchRoom(Room nextRoom)
    {
        CurrentRoom.gameObject.SetActive(false);
        nextRoom.gameObject.SetActive(true);
        
        CurrentRoom = nextRoom;
        // player.Added(nextRoom);
        
        FreezeLevel(roomTransitionTime);
    }
    
    public void SpawnPlayer()
    {
        player.gameObject.SetActive(true);
        player.OnSpawn();
    }
    
    public void FreezeLevel(float time)
    {
        Game.Pause();
        
        EffectManager.MoveCam(FindRoomStartCamPos(CurrentRoom), time).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            Game.Resume();
            player.OnResume();
        });
    }

    Vector2 FindRoomStartCamPos(Room room)
    {
        float x = room.BoundRect.position.x + 160;
        float y = room.BoundRect.position.y + 94;
        return new Vector2(x, y);
    }
} 
