
using System;
using DG.Tweening;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public float AliveTime = 0.26f;
    
    public Color StartColor;
    public Color EndColor; // alpha 0

    public float StartIntensity = 1.15f;
    public float EndIntensity = 0.4f;
    
    private SpriteRenderer sr;
    private Material mat;

    public void Init(bool facingRight, Sprite sprite)
    {
        if (TryGetComponent(out sr))
        {
            // has to flip if default dir is left sided
            sr.flipX = facingRight;
            sr.sprite = sprite;
            sr.color = StartColor;
            mat = sr.material;
            mat.SetFloat("_EmissionStrength", StartIntensity);
            
            DOTween.Sequence()
                .Append(sr.DOColor(EndColor, AliveTime).SetEase(Ease.InCubic))
                .Join(mat.DOFloat(EndIntensity, "_EmissionStrength", AliveTime).SetEase(Ease.InCubic))
                .OnComplete(() => Destroy(gameObject));
        }
    }
}


