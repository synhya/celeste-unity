
using UnityEngine;

public static class RectCustomUtil
{
    public static void PushOutsideOfBoundary(this ref Vector2Int target, Vector2Int min, Vector2Int max)
    {
        if (target.x > (min.x + max.x) / 2)
            target.x = Mathf.Max(target.x, min.x);
        else 
            target.x = Mathf.Min(target.x, max.x);
        
        if (target.y > (min.y + max.y) / 2)
            target.y = Mathf.Max(target.y, min.y);
        else 
            target.y = Mathf.Min(target.y, max.y);
    }
}


