using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game I =>
        (instance ??= (new GameObject("Manager")).AddComponent<Game>());
    private static Game instance = null;

    [SerializeField] private Level startingLevel;
    [SerializeField] private GameObject playerPrefab;
    
    [HideInInspector] public Level CurrentLevel;
    
    // 카메라는 레벨 객체별로 하나씩 존재하는게 맞다.
    // 메뉴화면이 3d인경우 또는 씬이 다른경우 완전히 다른 카메라를 쓸테니까.
    public static Camera MainCam => instance.cam;
    
    // 플레이어는 하나니까 일단 Don't destroy on load설정하고 
    // 레벨시작시마다 가져오는게 좋을듯
    public static Player MainPlayer => instance.mainPlayer;
    private Player mainPlayer;
    private Camera cam;

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
        
        CurrentLevel = startingLevel;

        // instance initialize
        cam = Camera.main;
        mainPlayer = Instantiate(playerPrefab).GetComponent<Player>();
    }

    void Start()
    {
        // set all levels to inactive.
        
        // Application.targetFrameRate = 144;
        StartGame();
    }
    
    void StartGame()
    {
        CurrentLevel.gameObject.SetActive(true);
        CurrentLevel.StartLevel();
    }
    
    public void Freeze(float time)
    {
        CurrentLevel.FreezeLevel(time);
    }
} 
