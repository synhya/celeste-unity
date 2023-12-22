using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Actor))]
public class ActorEditor : EntityEditor
{
}

[CustomEditor(typeof(Player))]
public class PlayerEditor : ActorEditor
{
}