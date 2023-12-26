
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
    private Level level;

    private DeathCircle[] circles;
    
    public void Init(Vector2 backDir, bool flipX)
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = LerpColor1;
        sr.flipX = !flipX; // dead sprite is opposite direction.
        
        var seq = DOTween.Sequence();
        var t = sr.transform;

        circles = new DeathCircle[8];

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
                    
                    circles[dir] = Instantiate(deathCircleObj, transform).GetComponent<DeathCircle>();
                        
                    circles[dir].transform.position = t.position + Vector3.up * circleYPosOffset;
                    circles[dir].Init(moveDir, LerpColor1, LerpColor2);
                    
                }
            })
            .InsertCallback(Max(respawnTime, circles[0].CircleAnimTime), () =>
            {
                // 나중에 서클 돌아오는 모션 추가
                // circles.. Loop backward
                
                level = Game.G.CurrentLevel;
                level.SpawnPlayer();
                
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



