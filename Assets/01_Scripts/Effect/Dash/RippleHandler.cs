
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

// shader has to be improved
public class RippleHandler : MonoBehaviour
{
    private Material mat;
    [SerializeField] private float manualPlaytime = 0.6f;
    [SerializeField] private float progressStart = -0.05f;
    [SerializeField] private float progressEnd = 0.28f;

    private void Awake()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }
    
    public void Ripple(Vector2 pos)
    {
        Ripple(pos, manualPlaytime);
    }
    
    public void Ripple(Vector2 pos, float time) 
    {
        // pos to uv
        Vector2 screenPos = EffectManager.mainCam.WorldToScreenPoint(pos);
        
        mat.SetVector("_SpawnScreenPos", screenPos);
        mat.DOFloat(progressEnd, "_Progress", time )
            .OnComplete(() => mat.SetFloat("_Progress", progressStart)); // max is 0.28
    }
}


