using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Solid), true)]
[CanEditMultipleObjects]
public class SolidEditor : EntityEditor
{
}

[CustomEditor(typeof(Spring), true)]
[CanEditMultipleObjects]
public class SpringEditor : SolidEditor
{
}
