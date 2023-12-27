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
    
    private DeadCirclePool dCirclePool;
    private DustPool dustPool;
    private DashLinePool dashLinePool;

    public DustVisualization GetDust() => dustPool.Pool.Get();
    public DashLineVisualization GetDashLine() => dashLinePool.Pool.Get();
    public DeathCircle GetCircle() => dCirclePool.Pool.Get();
    
    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);

        dCirclePool = GetComponent<DeadCirclePool>();
        dustPool = GetComponent<DustPool>();
        dashLinePool = GetComponent<DashLinePool>();
    }
} 
