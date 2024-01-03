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
        }
    }

    private void OnPreSceneGUI()
    {
        var r = target as Room;

        if (r)
        {
            boundHandle.size = (Vector2)r.BoundRect.size;
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
            Handles.color = Color.green;
            Handles.DrawSolidDisc( (Vector3Int) (bottomLeft + r.SpawnPosWS), new Vector3(0, 0, -1), 2f);
            
            // bound handle
            Handles.color = new Color(0.52f, 1f, 0.2f);
            boundHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;
            boundHandle.DrawHandle();
        
            // room links
            Handles.color = Color.red;
            for (int i = 0; i < r.RoomLinks.Length; i++)
            {
                Vector2Int start = bottomLeft, end = bottomLeft;
                var door = r.RoomLinks[i].Door;
                switch (door.Dir)
                {
                    case DoorDirections.Up:
                        start.x += door.StartPosWS;
                        start.y += r.BoundRect.size.y;
                        end = start;
                        end.x += door.Length;
                        break;
                    case DoorDirections.Down:
                        start.x += door.StartPosWS;
                        end = start;
                        end.x += door.Length;
                        break;
                    case DoorDirections.Right:
                        start.x += r.BoundRect.size.x;
                        start.y += door.StartPosWS;
                        end = start;
                        end.y += door.Length;
                        break;
                    case DoorDirections.Left:
                        start.y += door.StartPosWS;
                        end = start;
                        end.y += door.Length;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Handles.DrawLine((Vector2)start, (Vector2)end);
                Handles.DrawSolidDisc((Vector2)door.TransPosWS + bottomLeft, new Vector3(0, 0, -1), 2f);
            }
        
        
            // snap position to int
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                // bottom.y 는 4(2x + 1), top.y 는 4(2x)의 배수 -> 중심은 4x + 2
                var t = r.transform;
                var posX = Mathf.RoundToInt(t.position.x / 4) * 4;
                
                var bottomY = Mathf.RoundToInt(bottomLeft.y / 2f) * 2;
                var topY = Mathf.RoundToInt(topRight.y / 2f) * 2;
                
                while (bottomY % 8 != 4)
                {
                    bottomY += 2;
                }
                while (topY % 8 != 0)
                {
                    topY += 2;
                }
                var posY = (bottomY + topY) / 2f;
                
                t.position = new Vector3(posX, posY, 0);
                
                var spawnY = Mathf.RoundToInt(r.SpawnPosWS.y / 4f) * 4;
                if (spawnY % 8 != 4)
                    spawnY += 4;
                r.SpawnPosWS.y = spawnY;
            
                // bound save
                var sizeX = Mathf.RoundToInt(r.BoundRect.size.x / 8f) * 8;
                // sizeY는 8x + 4
                var sizeY = Mathf.RoundToInt(r.BoundRect.size.y / 4f) * 4;
                if (sizeY % 8 != 4)
                    sizeY += 4;
                r.BoundRect.size = new Vector2Int(sizeX, sizeY);
            }
        }
    }
    
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        
        base.OnInspectorGUI();

        var r = target as Room;
        if (r && EditorGUI.EndChangeCheck())
        {
            r.BoundRect.position = Vector2Int.zero; 
        }
        
        if (r && Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown)
        {
            // snap spawn pos
            var spawnY = Mathf.RoundToInt(r.SpawnPosWS.y / 4f) * 4;
            if (spawnY % 8 != 4)
                spawnY += 4;
            r.SpawnPosWS.y = spawnY;
            
            // snap door
            for (int i = 0; i < r.RoomLinks.Length; i++)
            {
                r.RoomLinks[i].Door.Length = Mathf.RoundToInt(r.RoomLinks[i].Door.Length / 8f) * 8;
                r.RoomLinks[i].Door.StartPosWS = Mathf.RoundToInt(r.RoomLinks[i].Door.StartPosWS / 4f) * 4;
            }
            
            // bound save
            var sizeX = Mathf.RoundToInt(r.BoundRect.size.x / 8f) * 8;
            // sizeY는 8x + 4
            var sizeY = Mathf.RoundToInt(r.BoundRect.size.y / 4f) * 4;
            if (sizeY % 8 != 4)
                sizeY += 4;
            r.BoundRect.size = new Vector2Int(sizeX, sizeY);
        }
    }
}
