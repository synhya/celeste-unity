
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

// shader has to be improved
public class RippleHandler : MonoBehaviour
{
    private Material mat;
    [SerializeField] private float manualPlaytime = 0.4f;
    [SerializeField] private float progressStart = -0.02f;
    [SerializeField] private float progressEnd = 0.2f;

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
        Vector2 screenPos = Game.MainCam.WorldToScreenPoint(pos);
        
        mat.SetVector("_SpawnScreenPos", screenPos);
        mat.DOFloat(progressEnd, "_Progress", time)
            .OnComplete(() => mat.SetFloat("_Progress", progressStart)); // max is 0.28
    }
}


