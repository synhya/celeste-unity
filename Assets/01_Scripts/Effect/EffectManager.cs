using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance => instance;
    private static EffectManager instance = null;
    
    public DustVisualization Dust;

    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
    }
} 
