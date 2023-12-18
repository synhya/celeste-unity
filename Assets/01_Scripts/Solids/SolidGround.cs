
using UnityEngine;
using UnityEngine.Tilemaps;

public class SolidGround : Solid
{
    public new void Move(float x, float y)
    {
        Debug.Log("Ground Can't Move");
    }
}


