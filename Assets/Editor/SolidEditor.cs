using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Solid))]
public class SolidEditor : Editor
{
    void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        Solid e = target as Solid;
       
       if(e == null || e.gameObject == null) return;
       
      e.PositionWS = Vector2Int.RoundToInt(e.transform.position);
      var rect = new Rect
      {
          x = e.PositionWS.x + e.HitboxBottomLeftOffset.x,
          y = e.PositionWS.y + e.HitboxBottomLeftOffset.y,
          width = e.HitboxSize.x,
          height = e.HitboxSize.y
      };
      
       Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.green);
    }
}
