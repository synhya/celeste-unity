
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FadePlatform))]
[CanEditMultipleObjects]
public class FadePlatformEditor : SolidEditor
{
    public override void OnSceneGUI()
    {
        base.OnSceneGUI();
        
        // instantiate go with length value of class FadePlatform..
        
        if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown)
        {

        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown)
        {
            
        }
    }

    void GenerateBlocks()
    {
        // get length
        
    }
}