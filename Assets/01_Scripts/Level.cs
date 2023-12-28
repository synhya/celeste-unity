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
    public GameObject PlayerPrefab;
    [SerializeField] private Room startingRoom;
    
    /// <summary>
    /// set on awake
    /// </summary>
    [HideInInspector] public Room CurrentRoom;
    [HideInInspector] public Player Player;
    [HideInInspector] public Tilemap Map;

    public bool InSpace = false;
    private const float RoomSwitchTime = 0.8f;
    public HashSet<Actor> AllActors;
    
    private Camera cam;

    void Awake() 
    {
        CurrentRoom = startingRoom;
        cam = Camera.main;
        
        Map = GetComponent<Tilemap>();
        Map.CompressBounds();
        
        AllActors = new HashSet<Actor>();
    }

    private void Start()
    {
        SpawnPlayer();
        
        startingRoom.gameObject.SetActive(true);
        float x = startingRoom.OriginWS.x + 160;
        float y = startingRoom.OriginWS.y + 94;
        cam.transform.position = new Vector3(x, y, -10);
    }

    public void SwitchRoom(Room nextRoom)
    {
        CurrentRoom.gameObject.SetActive(false);
        nextRoom.gameObject.SetActive(true);
        
        CurrentRoom = nextRoom;
        Player.OnSwitchRoomStart(nextRoom);
        
        FreezeLevel(RoomSwitchTime);
    }
    
    /// <summary>
    /// 플레이어는 항상 레벨아래에 위치하기 때문에 스폰도 룸이 아닌 레벨단위가 맞다.
    /// </summary>
    public void SpawnPlayer()
    {
        Player = Instantiate(PlayerPrefab, (Vector3Int)CurrentRoom.SpawnPos, 
            quaternion.identity).GetComponent<Player>();
    }
    
    public void FreezeLevel(float time)
    {
        Game.Pause();
        
        // adjust camera
        float x = CurrentRoom.OriginWS.x + 160;
        float y = CurrentRoom.OriginWS.y + 94;
        cam.transform.DOMove(new Vector3(x, y, -10), time)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                Game.Resume();
                Player.OnSwitchRoomEnd();
            });
    }

    public void Shake(float duration, float strength)
    {
        cam.DOShakePosition(duration, strength);
    }
    
    public void DirectionalShake(Vector2 dashDir, float duration, float strength)
    {
        // TODO: shake to direction
        cam.DOShakePosition(duration, strength);
    }
} 
