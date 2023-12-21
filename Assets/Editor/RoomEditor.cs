using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
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
            Handles.color = Color.green;
            Handles.DrawSolidDisc(r.RespawnPos, new Vector3(0, 0, -1), 2f);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
