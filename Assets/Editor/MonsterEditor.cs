using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Monster), true)]
[CanEditMultipleObjects]
public class MonsterEditor : EntityEditor
{
    public override void OnSceneGUI()
    {
        base.OnSceneGUI();

        var t = target as Monster;

        if (t)
        {
            var pos = t.transform.position;
            pos.x += t.PatrolRangeX;
            // show patrol points
            Handles.color = Color.cyan;
            Handles.DrawSolidDisc(pos, Vector3Int.forward, 1f);
            pos.x -= t.PatrolRangeX * 2;
            Handles.DrawSolidDisc(pos, Vector3Int.forward, 1f);

            var noticeRect = HitBoxRect;
            noticeRect.x = noticeRect.center.x - t.NoticeExtentX;
            noticeRect.width = t.NoticeExtentX * 2;
            Handles.DrawSolidRectangleWithOutline(noticeRect, Color.clear, Color.magenta);
        }
    }

}
