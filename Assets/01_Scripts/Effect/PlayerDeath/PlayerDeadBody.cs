
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

    [SerializeField] private Color normalColor = Color.red;
    [SerializeField] private Color dashCOlor = Color.cyan;

    private Color lerpColor1 = Color.white;
    private Color lerpColor2 = Color.red;
    
    private float circlePosOffsetY;

    private SpriteRenderer sr;

    public AudioSource Source;
    public SoundDataManager Sound => SoundDataManager.I;
    
    // 최적화 필요.
    public void Init(Vector2 backDir, bool flipX)
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = Color.white;
        sr.flipX = !flipX; // dead sprite is opposite direction.

        lerpColor2 = Game.MainPlayer.Dashes > 0 ? normalColor : dashCOlor;

        var t = transform;
        var srT = sr.transform;
        circlePosOffsetY = -srT.localPosition.y;
        
        var seq = DOTween.Sequence();
        seq.Append(sr.DOColor(lerpColor2, 0.5f).SetLoops(CeilToInt(BackTime / 0.5f), LoopType.Yoyo))
            .Join(srT.DOScale(Vector3.one * 0.5f, BackTime))
            .Join(t.DOMoveX(backDir.x * BackAmount.x, BackTime, true).SetRelative().SetEase(Ease.InCubic))
            .Join(t.DOMoveY(backDir.y * BackAmount.y, BackTime, true).SetRelative().SetEase(Ease.OutCubic))
            .InsertCallback(BackTime, () =>
            {
                sr.enabled = false;
                
                Sound.Play(Source, Sound.playerDeathSndData);
                Source.Play();
                
                for (var dir = 0; dir <= 7; dir++)
                {
                    var angle = dir / 4f * PI;
                    var moveDir = new Vector2(Cos(angle), Sin(angle));

                    var circle = EffectManager.GetCircle();
                    circle.Play(transform, circlePosOffsetY, moveDir, lerpColor1, lerpColor2);
                }
            })
            .InsertCallback(2f, () =>
            {
                Source.Stop();
                Sound.Play(Source, Sound.playerReviveSndData);
                Source.Play();
            })
            .InsertCallback(6f, () =>
            {
                Destroy(gameObject);
            });
    }
}



