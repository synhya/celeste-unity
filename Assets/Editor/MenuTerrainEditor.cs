using UnityEditor;
using UnityEngine;
using Random = System.Random;

[CustomEditor(typeof(MenuTerrain))]
public class MenuTerrainEditor : Editor
{

    public override void  OnInspectorGUI()
    {
        var t = target as MenuTerrain;
        
        if (GUILayout.Button("Generate"))
        {
            t.Initiate();
        }
        
        if (GUILayout.Button("Save Terrains"))
        {
            t.SaveMenuTerrains();
        }
    }
}


