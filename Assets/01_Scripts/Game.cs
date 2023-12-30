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
    [HideInInspector] public Level CurrentLevel;

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
        
        // set level when?
        CurrentLevel = startingLevel;
    }
    
    // onclick -> menu button invoke this
    void StartGame()
    {
        // disable all levels except starting one
    }

    void StartLevel()
    {
        // disable all rooms except starting one
    }

    public void Freeze(float time)
    {
        CurrentLevel.FreezeLevel(time);
    }
} 
