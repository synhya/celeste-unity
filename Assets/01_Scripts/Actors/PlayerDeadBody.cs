
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
    public float KnockBackAmount = 3f;
    public float KnockBackTime = 2f;
    public float KnockBackStrength = 2f;
    
    [Header("Color Settings")]
    public Color LerpColor1 = Color.white;
    public Color LerpColor2 = Color.red;

    [Header("Other Settings")]
    [SerializeField] private float respawnTime = 8f;
    [SerializeField] private float circleYPosOffset = 2.5f;
    [SerializeField] private GameObject deathCircleObj;

    private SpriteRenderer sr;
    
    public void Init(Vector2 backDir, bool flipX)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = LerpColor1;
        sr.flipX = !flipX; // dead sprite is opposite direction.
        
        var seq = DOTween.Sequence();
        var t = transform;

        seq.Append(t.DOScale(Vector3.one * 0.5f, KnockBackTime))
            .Join(t.DOJump(t.position + ((Vector3)backDir * KnockBackAmount), KnockBackStrength, 
                1, KnockBackTime).SetEase(Ease.OutCubic))
            .InsertCallback(KnockBackTime, () =>
            {
                sr.enabled = false;
                for (var dir = 0; dir <= 7; dir++)
                {
                    var angle = dir / 4f * PI;
                    var moveDir = new Vector2(Cos(angle), Sin(angle));
                    
                    var deadCircle = Instantiate(deathCircleObj, t.position + Vector3.up * circleYPosOffset,quaternion.identity)
                        .GetComponent<DeathCircle>();
                    deadCircle.Init(moveDir, LerpColor1, LerpColor2);
                }
            })
            .InsertCallback(respawnTime, () =>
            {
                // respawn -> need respawn transform -> each room has respawn point
                var game = GameManager.I;
                game.SpawnPlayer();
                
                Destroy(gameObject);
            });
    }
    private void Update()
    {
        // lerp color
        sr.color = Color.Lerp(LerpColor1, LerpColor2, 
            PingPong(Time.time * 2f, 1));
    }
}



