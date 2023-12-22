using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(Room))]
[CanEditMultipleObjects]
public class RoomEditor : Editor
{
    void OnEnable()
    {
        
    }

    private void OnSceneGUI()
    {
        var r = target as Room;

        // r.RespawnPos;

        if (r)
        {
            // r.StaticTilemap = r.GetComponent<Tilemap>();
            // r.StaticTilemap.CompressBounds();
            // var originWS = (Vector2Int)r.StaticTilemap.origin * (int)r.StaticTilemap.cellSize.x;

            var originWS = Vector2Int.RoundToInt(r.transform.position);
            r.SpawnPos = Vector2Int.Max(r.SpawnPos, originWS);
            
            Handles.color = Color.green;
            Handles.DrawSolidDisc((Vector3Int)r.SpawnPos, new Vector3(0, 0, -1), 2f);
            
            // draw 320, 184 from origin
            var rect = new Rect(originWS, new Vector2(320, 184));
            Handles.color = Color.cyan;
            Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.cyan);
            
            Handles.color = Color.yellow;
            for (int i = 0; i < r.Doors.Length; i++)
            {
                rect = new Rect(r.Doors[i].position + originWS, r.Doors[i].size);
                Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.yellow);
            }
            
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
