
using System;
using DG.Tweening;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public float AliveTime;
    public Color EndColor; // alpha 0
    
    private SpriteRenderer sr;

    public void Init(bool facingRight)
    {
        if (TryGetComponent(out sr))
        {
            // has to flip if default dir is left sided
            sr.flipX = facingRight;
            sr.DOColor(EndColor, AliveTime).OnComplete(
                () => Destroy(gameObject));
        }
    }
}


