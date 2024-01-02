using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(Room))]
[CanEditMultipleObjects]
public class RoomEditor : Editor
{
    private BoxBoundsHandle boundHandle;

    private void OnEnable()
    {
        var r = target as Room;

        if (r)
        {
            var position = r.transform.position;
            position = Vector3Int.RoundToInt(position);
            r.transform.position = position;

            boundHandle = new BoxBoundsHandle();
            boundHandle.size = (Vector2)r.BoundRect.size;
            boundHandle.center = (Vector2)position + (r.BoundRect.size / 2);

            if (r.TransitionPos == Vector2Int.zero)
            {
                r.TransitionPos = Vector2Int.RoundToInt(boundHandle.center);
            }
        }
    }

    private void OnPreSceneGUI()
    {
        var r = target as Room;

        if (r)
        {
            boundHandle.center = Vector3Int.RoundToInt(r.transform.position);
        }
    }

    private void OnSceneGUI()
    {
        var r = target as Room;

        if (r)
        {
            var bottomLeft = Vector2Int.RoundToInt(boundHandle.center - boundHandle.size / 2);
            var topRight = Vector2Int.RoundToInt(boundHandle.center + boundHandle.size / 2);
            
            // spawn position
            r.SpawnPos = Vector2Int.Max(r.SpawnPos, bottomLeft);
            r.SpawnPos = Vector2Int.Min(r.SpawnPos, topRight);
            Handles.color = Color.green;
            Handles.DrawSolidDisc((Vector3Int)r.SpawnPos, new Vector3(0, 0, -1), 2f);
        
            // transition target position
            r.TransitionPos.PushOutsideOfBoundary(bottomLeft - Vector2Int.one, topRight + Vector2Int.one);

            Handles.color = Color.cyan;
            Handles.DrawSolidDisc((Vector3Int)r.TransitionPos, new Vector3(0, 0, -1), 2f);

            // bound handle
            Handles.color = new Color(0.52f, 1f, 0.2f);
            boundHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;
            boundHandle.DrawHandle();
        
            // doors
            Handles.color = Color.red;
            for (int i = 0; i < r.RoomLinks.Length; i++)
            {
                Vector2 start = bottomLeft, end = bottomLeft;
                var door = r.RoomLinks[i].Door;
                switch (door.Dir)
                {
                    case DoorDirections.Up:
                        start.x += door.StartPos;
                        start.y += r.BoundRect.size.y;
                        end = start;
                        end.x += door.Length;
                        break;
                    case DoorDirections.Down:
                        start.x += door.StartPos;
                        end = start;
                        end.x += door.Length;
                        break;
                    case DoorDirections.Right:
                        start.x += r.BoundRect.size.x;
                        start.y += door.StartPos;
                        end = start;
                        end.y += door.Length;
                        break;
                    case DoorDirections.Left:
                        start.y += door.StartPos;
                        end = start;
                        end.y += door.Length;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Handles.DrawLine(start, end);
            }
        
        
            // snap position to int
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                // round by 8
                r.transform.position = Vector3Int.RoundToInt(r.transform.position / 4) * 4;
                r.SpawnPos.y = (r.SpawnPos.y / 8) * 8;
            
                // bound save
                r.BoundRect.size = Vector2Int.RoundToInt(boundHandle.size);
            }
        }
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var r = target as Room;
        
        if (r && Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown)
        {
            // snap spawn pos
            var pos = r.SpawnPos;
            pos.y = Mathf.RoundToInt(pos.y / 8f) * 8;
            r.SpawnPos = pos;
            
            // snap door
            for (int i = 0; i < r.RoomLinks.Length; i++)
            {
                r.RoomLinks[i].Door.Length = Mathf.RoundToInt(r.RoomLinks[i].Door.Length / 8f) * 8;
                r.RoomLinks[i].Door.StartPos = Mathf.RoundToInt(r.RoomLinks[i].Door.StartPos / 8f) * 8;
            }
        }
    }
    
    // private void RoomLinkCheck(Room r)
    // {
    //     // if next room exists -> link
    //     var p = r.transform.parent;
    //     var curIdx = r.transform.GetSiblingIndex() + 1;
    //     Transform t = p.childCount - 1 >= curIdx + 1 ? p.GetChild(curIdx + 1) : null;
    //     if (t)
    //     {
    //         r.NextRooms ??= new Room[1];
    //         if(t.TryGetComponent(out Room nextR))
    //             r.NextRooms[0] ??= nextR;
    //     }
    // }
    //
}
