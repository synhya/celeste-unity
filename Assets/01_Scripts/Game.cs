using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game G =>
        (instance ??= (new GameObject("Manager")).AddComponent<Game>());
    private static Game instance = null;

    [SerializeField] private Level startingLevel;
    [HideInInspector] public Level CurrentLevel;

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

    public void Freeze(float time)
    {
        CurrentLevel.FreezeLevel(time);
    }
} 
