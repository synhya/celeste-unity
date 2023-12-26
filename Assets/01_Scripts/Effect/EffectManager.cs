using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// contains pools for effects
/// </summary>
public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance => instance;
    private static EffectManager instance = null;


    [FormerlySerializedAs("GameObjectPool")]
    public GameObjectPool DustPool;
    private DustVisualization dust;

    public GameObjectPool DashLinePool;
    private DashLineVisualization dashLine;

    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
    }

    public DustVisualization GetDust()
    {
        return DustPool.Pool.Get().GetComponent<DustVisualization>();
    }
} 
