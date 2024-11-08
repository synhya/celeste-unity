using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private DashTrailPool dashTrailPool;

    [SerializeField] private RippleHandler rippleHandler;

    public static DustVisualization GetDust() => instance.dustPool.Pool.Get();
    public static DashLineVisualization GetDashLine() => instance.dashLinePool.Pool.Get();
    public static DeathCircle GetCircle() => instance.dCirclePool.Pool.Get();
    public static DashTrail GetTrail() => instance.dashTrailPool.Pool.Get();
    public static void CreateRipple(Vector2 pos) => instance.rippleHandler.Ripple(pos);
    
    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
        
        // get components
        dCirclePool = GetComponent<DeadCirclePool>();
        dustPool = GetComponent<DustPool>();
        dashLinePool = GetComponent<DashLinePool>();
        dashTrailPool = GetComponent<DashTrailPool>();
    }

    #region Camera effects
    
    public static Tweener ShakeCam(float duration, float strength) 
        => Level.Current.CamShakerT.DOShakePosition(duration, strength);
    
    #endregion
}