using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Solid), true)]
[CanEditMultipleObjects]
public class SolidEditor : EntityEditor
{
}

[CustomEditor(typeof(Trigger), true)]
[CanEditMultipleObjects]
public class TriggerEditor : EntityEditor
{
}

