using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class Game : MonoBehaviour
{
    public static Game I =>
        (instance ??= (new GameObject("Manager")).AddComponent<Game>());
    private static Game instance = null;
    
    public const int Width = 320;
    public const int Height = 180;
    
    [SerializeField] private GameObject playerPrefab;
    public static Player MainPlayer => instance.mainPlayer;
    private Player mainPlayer;

    private AudioSource windSource;
    
    public static bool IsPaused
    {
        get;
        private set;
    }

    public static void Pause() => IsPaused = true;
    public static void Resume() => IsPaused = false;
    
    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);

        // instance initialize
        mainPlayer = Instantiate(playerPrefab).GetComponent<Player>();
        DontDestroyOnLoad(mainPlayer);
        mainPlayer.gameObject.SetActive(false);
    }

    private void Start()
    {
        windSource = GetComponent<AudioSource>();
        SoundDataManager.I.Play(windSource, SoundDataManager.I.menuWindSndData);
    }

    // void StartGame()
    // {
    //
    // }
} 
