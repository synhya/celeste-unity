
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class DashTrail : MonoBehaviour, IPoolable
{
    public float AliveTime = 0.26f;
    
    public Color StartColor;
    public Color EndColor; // alpha 0

    public float StartIntensity = 1.15f;
    public float EndIntensity = 0.4f;
    
    private SpriteRenderer sr;
    private Material mat;
    private IObjectPool<DashTrail> pool;

    public void Init<T>(IObjectPool<T> pool) where T : MonoBehaviour
    {
        this.pool = pool as IObjectPool<DashTrail>;

        sr = GetComponent<SpriteRenderer>();
        mat = sr.material;
    }

    public void Play(Vector3 posWS, bool facingRight, Sprite sprite)
    {
        transform.position = posWS;
        // has to flip if default dir is left sided
        sr.flipX = facingRight;
        sr.sprite = sprite;
        sr.color = StartColor;
        mat.SetFloat("_EmissionStrength", StartIntensity);
            
        DOTween.Sequence()
            .Append(sr.DOColor(EndColor, AliveTime).SetEase(Ease.InCubic))
            .Join(mat.DOFloat(EndIntensity, "_EmissionStrength", AliveTime).SetEase(Ease.InCubic))
            .OnComplete(() => pool.Release(this));
    }
}


