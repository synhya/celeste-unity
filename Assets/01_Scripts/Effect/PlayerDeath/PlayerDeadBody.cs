
using System;
using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

/// <summary>
/// circle thing that rotates around player when dead
/// </summary>
public class PlayerDeadBody : MonoBehaviour
{
    [Header("Knockback Anim Settings")]
    public Vector2 BackAmount = Vector2.one;
    public float BackTime = 0.5f;
    
    [Header("Color Settings")]
    public Color LerpColor1 = Color.white;
    public Color LerpColor2 = Color.red;
    
    private float circlePosOffsetY;

    private SpriteRenderer sr;
    
    private readonly EffectManager em = EffectManager.Instance;
    
    // 최적화 필요.
    public void Init(Vector2 backDir, bool flipX)
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = LerpColor1;
        sr.flipX = !flipX; // dead sprite is opposite direction.

        var t = transform;
        var srT = sr.transform;
        circlePosOffsetY = -srT.localPosition.y;
        
        var seq = DOTween.Sequence();
        seq.Append(sr.DOColor(LerpColor2, 0.5f).SetLoops(CeilToInt(BackTime / 0.5f), LoopType.Yoyo))
            .Join(srT.DOScale(Vector3.one * 0.5f, BackTime))
            .Join(t.DOMoveX(backDir.x * BackAmount.x, BackTime, true).SetRelative().SetEase(Ease.InCubic))
            .Join(t.DOMoveY(backDir.y * BackAmount.y, BackTime, true).SetRelative().SetEase(Ease.OutCubic))
            .InsertCallback(BackTime, () =>
            {
                sr.enabled = false;
                for (var dir = 0; dir <= 7; dir++)
                {
                    var angle = dir / 4f * PI;
                    var moveDir = new Vector2(Cos(angle), Sin(angle));

                    var circle = em.GetCircle();
                    circle.Play(transform, circlePosOffsetY, moveDir, LerpColor1, LerpColor2);
                }
            })
            .InsertCallback(6f, () =>
            {
                Destroy(gameObject);
            });
    }
}



