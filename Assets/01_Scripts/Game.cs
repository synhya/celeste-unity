using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{
    public static Game I =>
        (instance ??= (new GameObject("Manager")).AddComponent<Game>());
    private static Game instance = null;
    
    [SerializeField] private GameObject playerPrefab;

    public const int Width = 320;
    public const int Height = 180;
    
    public static Player MainPlayer => instance.mainPlayer;
    private Player mainPlayer;
    public static Level CurrentLevel => instance.currentLevel;
    private Level currentLevel;

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

    void Start()
    {
        // set all levels to inactive.
        
        // Application.targetFrameRate = 144;
        StartGame();
    }
    
    void StartGame()
    {
        // switch scene here.
        
        CurrentLevel.gameObject.SetActive(true);
        CurrentLevel.StartLevel();
    }
} 
