
using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Serialization;

public class HoodColorHandler : MonoBehaviour
{
    private Player player;
    private Material targetMat;

    [SerializeField] private Texture2D tableTex;

    [Header("Animation Settings")]
    [SerializeField] private float animTime = 0.15f;
    [SerializeField] private float maxIntensity = 1f;

    [SerializeField] [SerializedDictionary("from", "to")]
    private SerializedDictionary<Color32, Color32> colorToColorMap;
    private Dictionary<Vector2Int, PairColor> coordToColorMap;
    
    private float lerpValue;
    private bool isChangingColor;
    private bool hasChangedColor;
    private Sequence seq;

    private Color? forceColor = null;
    private static readonly int EmissionStrength = Shader.PropertyToID("_EmissionStrength");

    private void Awake()
    {
        coordToColorMap = new Dictionary<Vector2Int, PairColor>();
        
        var tex = tableTex;
        var texColors = tex.GetPixels32();
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                var originColor = texColors[i + j * tex.width];

                if (colorToColorMap.TryGetValue(originColor, out var swapColor))
                    coordToColorMap.Add(new Vector2Int(i, j), new PairColor(originColor, swapColor));
            }
        }
    }

    private void Start()
    {
        player = Game.MainPlayer;
        targetMat = player.SR.material;
        tableTex = Instantiate(tableTex); // in order to avoid overriding source tex.
        targetMat.SetTexture("_LookUpTex", tableTex);
    }

    public void OnDash()
    {
        isChangingColor = true;
        
        lerpValue = 0;
        targetMat.SetFloat(EmissionStrength, 0);
        seq.Kill();
        
        seq = DOTween.Sequence()
            .Append(DOTween.To(() => lerpValue, x => lerpValue = x, 1, animTime)
                .SetEase(Ease.OutCubic))
            .Join(targetMat.DOFloat(maxIntensity, EmissionStrength, animTime))
            .SetAutoKill(false);
        seq.OnComplete(() =>
        {
            if (player.Dashes > 0)
            {
                // better than blink
                seq.PlayBackwards();
            }
            else
            {
                isChangingColor = false;
                hasChangedColor = true;
            }
        });
    }
    
    private void Update()
    {
        // can be changed to state machine
        if (isChangingColor)
        {
            SwitchPixel(lerpValue, forceColor);
            tableTex.Apply();
        }
        else if (hasChangedColor && player.Dashes > 0)
        {
            hasChangedColor = false;
            isChangingColor = true;
            
            seq.PlayBackwards();
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="t">should range from 0 to 1</param>
    void SwitchPixel(float t, Color? forceColor)
    {
        foreach (var pair in coordToColorMap)
        {
            var coord = pair.Key;
            var color = forceColor ?? Color.Lerp(pair.Value.from, pair.Value.to, t);
            
            tableTex.SetPixel(coord.x, coord.y, color);
        }
    }

    void BlinkColor()
    {
        // white then red 
        forceColor = Color.white;
        targetMat.SetFloat(EmissionStrength, 0);
        DOTween.Sequence()
            .InsertCallback(0.2f, () =>
            {
                forceColor = null;
                lerpValue = 0;
            });
    }
    
    private void OnDisable()
    {
        // seq.Kill();
    }
}


