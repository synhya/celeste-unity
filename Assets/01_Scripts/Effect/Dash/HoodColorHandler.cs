
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Serialization;

public class HoodColorHandler : MonoBehaviour
{
    // texture to modify
    private Player player => Game.I.CurrentLevel.Player;
    private Material targetMat;

    [SerializeField] private Texture2D tableTex;
    [SerializeField] private Texture2D tableTexBackup;

    [Header("Animation Settings")]
    [SerializeField] private float animTime = 0.5f;
    [SerializeField] private float maxIntensity = 1f;

    [SerializeField] [SerializedDictionary("from", "to")]
    private SerializedDictionary<Color32, Color32> colorToColorMap;
    private Dictionary<Vector2Int, PairColor> coordToColorMap;
    
    private float lerpValue;
    private bool isChangingColor;
    private bool hasChangedColor;
    private TweenerCore<float, float, FloatOptions> lerpTween;

    private void Start()
    {
        coordToColorMap = new Dictionary<Vector2Int, PairColor>();
        foreach (var pair in colorToColorMap)
        {
            var value = pair.Value;
            ColorUtility.TryParseHtmlString("#"+ColorUtility.ToHtmlStringRGB(pair.Key), out var hexColor);
            colorToColorMap.Remove(pair.Key);
            colorToColorMap.Add(hexColor, value);
        }
        
        var tex = tableTex;
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                var originColor = tex.GetPixel(i, j);
                ColorUtility.TryParseHtmlString("#"+ColorUtility.ToHtmlStringRGB(originColor), out var hexColor);
                if (colorToColorMap.TryGetValue(hexColor, out var swapColor))
                    coordToColorMap.Add(new Vector2Int(i, j), new PairColor(originColor, swapColor));
            }
        }
        Debug.Log(coordToColorMap.Count);
    }

    public void OnDash()
    {
        isChangingColor = true;
        targetMat = player.SR.material;

        lerpTween.Kill();
        lerpValue = 0;
        lerpTween = DOTween.To(() => lerpValue, x => lerpValue = x, 1, animTime)
            .SetEase(Ease.OutCubic)
            .SetAutoKill(false);
        
        lerpTween.OnComplete(() =>
        {
            if (player.Dashes > 0)
                lerpTween.PlayBackwards();
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
            SwitchPixel(lerpValue);
            tableTex.Apply();
        }
        else if (hasChangedColor && player.Dashes > 0)
        {
            hasChangedColor = false;
            isChangingColor = true;
            lerpTween.PlayBackwards();
        }
        

        // // also change intensity value with targetMat
        // targetMat.SetFloat("_EmissionStrength", maxIntensity);
    }

    /// <summary>
    /// </summary>
    /// <param name="t">should range from 0 to 1</param>
    void SwitchPixel(float t)
    {
        foreach (var pair in coordToColorMap)
        {
            var coord = pair.Key;
            var color = Color.Lerp(pair.Value.from, pair.Value.to, t);
            
            tableTex.SetPixel(coord.x, coord.y, color);
        }
    }

    [ContextMenu("Rollback LUT")]
    void RollbackLUT()
    {
        var colors = tableTexBackup.GetPixels();
        tableTex.SetPixels(colors);
        tableTex.Apply();
    }

    private void OnDestroy()
    {
        // switch back to original texture
        SwitchPixel(0);
    }
}


