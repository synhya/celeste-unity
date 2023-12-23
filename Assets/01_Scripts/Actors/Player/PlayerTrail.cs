
using System;
using DG.Tweening;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public float AliveTime;
    public Color StartColor;
    public Color EndColor; // alpha 0

    public float StartIntensity = 1.69f;
    public float EndIntensity = 0.4f;
    
    private SpriteRenderer sr;
    private Material mat;
    private float aliveTimer;

    private bool didStart;

    private Color varColor;
    private float varIntensity;

    public void Init(bool facingRight, Sprite sprite)
    {
        if (TryGetComponent(out sr))
        {
            // has to flip if default dir is left sided
            sr.flipX = facingRight;
            sr.sprite = sprite;

            mat = sr.material;
            mat.SetColor("_TrailColor", StartColor);
            mat.SetFloat("_Intensity", 1f);

            aliveTimer = AliveTime;
            didStart = true;
        }
    }

    private void Update()
    {
        if (aliveTimer > 0f)
        {
            aliveTimer -= Time.deltaTime;
            var lerpValue = aliveTimer / AliveTime; // 1 -> 0
            varColor = Color.Lerp(EndColor, StartColor, lerpValue);
            varIntensity = Mathf.Lerp(EndIntensity, StartIntensity, lerpValue);
            mat.SetColor("_TrailColor", varColor);
            mat.SetFloat("_Intensity", varIntensity);
            
        } else if (didStart)
        {
            Destroy(gameObject);
        }
    }
}


