using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(Room))]
[CanEditMultipleObjects]
public class RoomEditor : Editor
{
    private void OnSceneGUI()
    {
        var r = target as Room;
        
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
        
        // snap position to int
        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            // round by 8
            r.transform.position = Vector3Int.RoundToInt(r.transform.position / 8) * 8;
            r.SpawnPos.y = (r.SpawnPos.y / 8) * 8;

            RoomLinkCheck(r);
        }
    }
    private void RoomLinkCheck(Room r)
    {
        // if next room exists -> link
        var p = r.transform.parent;
        var curIdx = r.transform.GetSiblingIndex() + 1;
        Transform t = p.childCount - 1 >= curIdx + 1 ? p.GetChild(curIdx + 1) : null;
        if (t)
        {
            r.NextRooms ??= new Room[1];
            if(t.TryGetComponent(out Room nextR))
                r.NextRooms[0] ??= nextR;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown)
        {
            // snap spawn pos
            var r = target as Room;
            
            var pos = r.SpawnPos;
            pos.y = Mathf.RoundToInt(pos.y / 8f) * 8;
            r.SpawnPos = pos;
            
            // snap door
            for (int i = 0; i < r.Doors.Length; i++)
            {
                r.Doors[i].position = Vector2Int.RoundToInt((Vector2)r.Doors[i].position / 8f) * 8;
            }
            
            RoomLinkCheck(r);
        }
    }
}
