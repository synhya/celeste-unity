
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class DeathCircle : MonoBehaviour, IPoolable
{
    [Header("Spread Settings")]
    [SerializeField] private float targetScale = 0.53f;
    [SerializeField] private float targetMoveSpeed = 15f;
    [SerializeField] private float moveTime = 1.2f;

    [Header("Spin Settings")]
    [SerializeField] private float angleSpeed = 120f;

    [Header("Shrink Settings")]
    [SerializeField] private float shrinkTime = 0.8f;

    [SerializeField] private float intensityInRed = 2;
    [SerializeField] private float intensityInWhite = 1;

    private SpriteRenderer sr;
    private IObjectPool<DeathCircle> pool;
    private Transform originalParent;

    private static bool didResetPivotPos;
    private static bool didSpawnPlayer;

    [HideInInspector] public float TotalTime;
    
    public void Init<T>(IObjectPool<T> pool) where T : MonoBehaviour
    {
        sr = GetComponent<SpriteRenderer>();
        this.pool = pool as IObjectPool<DeathCircle>;
        originalParent = transform.parent;
    }
    
    public void Play(Transform pivotT, float circleYOffset, Vector2 moveDir, Color color1, Color color2)
    {
        didResetPivotPos = false;
        didSpawnPlayer = false;
        
        var t = transform;
        
        t.SetParent(pivotT, false);
        t.localPosition = Vector3.zero;
        sr.color = color1;
        sr.material.SetFloat("_EmissionStrength", intensityInWhite);
        TotalTime = moveTime + shrinkTime;

        var loopCount = Mathf.CeilToInt(TotalTime / 0.5f);
        
        // seq goes outward ans scale bigger
        var seq = DOTween.Sequence()
            .Append(sr.DOColor(color2, 0.5f).SetLoops(loopCount, LoopType.Yoyo))
            .Join(sr.material.DOFloat(intensityInRed, "_EmissionStrength", 0.5f).SetEase(Ease.OutCubic)
                .SetLoops(loopCount, LoopType.Yoyo))
            .Join(pivotT.DOLocalRotate(Vector3.forward * (angleSpeed * TotalTime), TotalTime).SetRelative())
            .Join(t.DOScale(targetScale, moveTime))
            .Join(t.DOLocalMove(moveDir * (targetMoveSpeed * moveTime), moveTime).SetRelative())
            .Insert(moveTime, t.DOScale(Vector3.zero, shrinkTime).SetEase(Ease.OutExpo))
            .SetAutoKill(false);
        seq.InsertCallback(TotalTime * 0.75f,() =>
        {
            if (!didResetPivotPos)
            {
                didResetPivotPos = true;
                var spawnP = Level.Current.CurRoom.SpawnPosWS;
                Game.MainPlayer.PositionWS = spawnP;
                spawnP.y += (int)circleYOffset;
                pivotT.position = (Vector2)spawnP;
            }
            seq.PlayBackwards();
        });
        seq.OnRewind(() =>
        {
            if (!didSpawnPlayer)
            {
                didSpawnPlayer = true;
                Level.Current.ReSpawnPlayer();
            }
            t.SetParent(originalParent);
            seq.Kill();
            pool.Release(this);
        });
    }
}


