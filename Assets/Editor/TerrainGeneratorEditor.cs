
using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CustomEditor(typeof(TerrainGenerator))]
[CanEditMultipleObjects]
public class TerrainGeneratorEditor : Editor
{
    private SerializedProperty property;
    private bool useAdditionalTexture;
    private void OnEnable()
    {
        property = serializedObject.FindProperty("HeightTexture");
    }

    private void OnSceneGUI()
    {
        var t = target as TerrainGenerator;
        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            t.Initiate();
        }
    }
    
    public override void OnInspectorGUI()
    {
        var t = target as TerrainGenerator;

        if (DrawDefaultInspector())
        {
            t.Initiate();
        }

        if (GUILayout.Button("New Seed"))
        {
            t.seed = Random.Range(0, 100);
            t.Initiate();
        }

        if (GUILayout.Button("Save Mesh as Asset"))
        {
            t.SaveMesh();
        }
        
        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            t.Initiate();
        }
    }
}


