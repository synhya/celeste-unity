
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Actor), true)]
[CanEditMultipleObjects]
public class ActorEditor : EntityEditor
{
}

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

