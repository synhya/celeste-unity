using System;
using System.Collections.Generic;
using DG.Tweening;
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
    public static Level Current => instance;
    private static Level instance;
    
    // for intro
    [Header("Intro")]
    [SerializeField] private Material introMat;
    [SerializeField] private Vector2 cropScreenPos;
    [SerializeField] private float targetProgress; // from 1 to backward
    
    [Header("General")]
    [SerializeField] private Room startingRoom;
    [HideInInspector] public Room CurRoom;
    private Room prevRoom;
    [HideInInspector] public Tilemap Map;

    public HashSet<Actor> AllActors;
    private Player player;
    
    public IntroTypes IntroType = IntroTypes.Jump;
    
    private Transform camT;

    [HideInInspector] public RoomLink NextRoomLink;

    private Vector2 curBoundMin;
    private Vector2 curBoundMax;
    private Vector2 boundOffset = new Vector2(Game.Width / 2f,  Game.Height / 2f);
    
    
    [HideInInspector] public Camera MainCam;
    [HideInInspector] public Transform CamShakerT;
    
    void Awake()
    {
        // 레벨 바뀔때 바꿔야되니까.
        if(instance != null) Destroy(instance.gameObject);
        instance = this;
        
        CurRoom = startingRoom;
        Map = GetComponent<Tilemap>();
        Map.CompressBounds();
        AllActors = new HashSet<Actor>();

        foreach (Transform room in transform)
        {
            if(room.gameObject.name.Contains("Room"))
                room.gameObject.SetActive(false);
        }

        MainCam = Camera.current;
        camT = MainCam.transform;
        CamShakerT = camT.parent;
    }

    public void Start()
    {
        CurRoom.gameObject.SetActive(true); // 이러면 awake먼저 실행됨 이함수 도중에!
        
        player = Game.MainPlayer;
        var pt = player.transform;
        pt.SetParent(transform);
        SpawnPlayer();
        
        curBoundMin = CurRoom.BoundRect.min + boundOffset;
        curBoundMax = CurRoom.BoundRect.max - boundOffset;
        
        camT.position = CameraTarget;
        
        // intro
        introMat.SetVector("_CenterScreenPos", cropScreenPos);
        introMat.SetFloat("_Progress", 1);
        introMat.DOFloat(targetProgress, "_Progress", 1f)
            .OnComplete(() =>
            {
                // player intro
            }); // after intro wipe out circle.
    }

    private void Update()
    {
        // shake하고 움직여서 느려지는거였다.
        var from = camT.position;
        camT.position
            = from + (CameraTarget - from) * (1f - Mathf.Pow(0.01f, Time.deltaTime));
        
        // for each frame, check if player needs transition or is dead(out of boundary)
        if(player.State != Player.StateTransition && !player.Dead)
        {
            // up 
            if (player.UpWS > CurRoom.BoundRect.yMax)
                CheckDoorsAndMove(player.RightWS, player.LeftWS, DoorDirections.Up);
            // down
            else if (player.DownWS < CurRoom.BoundRect.yMin && 
                     !CheckDoorsAndMove(player.RightWS, player.LeftWS, DoorDirections.Down))
                player.Die(Vector2.up);               
            // side
            else if (player.LeftWS < CurRoom.BoundRect.xMin)
                CheckDoorsAndMove(player.DownWS, player.UpWS, DoorDirections.Left);
            else if (player.RightWS > CurRoom.BoundRect.xMax)
                CheckDoorsAndMove(player.DownWS, player.UpWS, DoorDirections.Right);
        }
    }
    
    bool CheckDoorsAndMove(int min, int max, DoorDirections dir)
    {
        foreach (var link in CurRoom.RoomLinks)
        {
            var door = link.Door;

            if (dir == door.Dir)
            {
                // overlap
                if (min >= door.StartPosWS &&
                    max <= door.StartPosWS + door.Length)
                {
                    StartTransitionTo(link);
                    return true;
                }
            }
        }
        return false;
    }

    private void StartTransitionTo(RoomLink link)
    {
        NextRoomLink = link;   
        prevRoom = CurRoom;
        CurRoom = link.Room;
        CurRoom.gameObject.SetActive(true);
        
        player.State = Player.StateTransition; // -> transitionBegin
        
        curBoundMin = CurRoom.BoundRect.min + boundOffset;
        curBoundMax = CurRoom.BoundRect.max - boundOffset;
    }

    public void OnTransitionEnd()
    {
        prevRoom.gameObject.SetActive(false);
    }

    /// <summary>
    /// should follow player and be clamped to room boundary.
    /// </summary>
    private Vector3 CameraTarget
    {
        get {
            Vector3 at = new Vector3(0 ,0, -10);
            Vector2 target = player.PositionWS;
            
            
            at.x = Mathf.Clamp(target.x, curBoundMin.x, curBoundMax.x);
            at.y = Mathf.Clamp(target.y, curBoundMin.y, curBoundMax.y);

            return at;
        }
    }
    
    public void SpawnPlayer()
    {
        player.gameObject.SetActive(true);
        player.PositionWS = CurRoom.SpawnPosWS;
        player.OnSpawn();
    }
} 
