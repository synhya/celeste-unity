using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
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
        Player e = target as Player;
       
        if(e == null || e.gameObject == null) return;
        
        if (Application.isPlaying)
        {
            // var rect = new Rect(e.HitBoxWS.x, e.HitBoxWS.y, e.HitBoxWS.width, e.HitBoxWS.height);
            // rect.x += Mathf.Sign(Mathf.RoundToInt(e.Remainder.x));
            // e.UpdateGridPos();
            // Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.green);
            //
            // for(int i = e.GridPosMin.x; i <= e.GridPosMax.x; i++)
            // {
            //     for (int j = e.GridPosMin.y; j <= e.GridPosMax.y; j++)
            //     {
            //         var pos = new Vector3Int(i, j, 0);
            //         if (e.Room.Tilemap.HasTile(pos))
            //         {
            //             rect.center = e.Room.Tilemap.GetCellCenterWorld(pos);
            //             rect.size = new Vector2(8, 8);
            //
            //             Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.blue);
            //         }
            //     }
            // }
            //
            // rect = new Rect(e.TileRect.x, e.TileRect.y, e.TileRect.width, e.TileRect.height);
            // Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.red);
        }
        else
        {
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
}
