
using System;
using UnityEngine;

public class Platform : Solid
{
    public int GearOffset = 8;

    [SerializeField] private float maxSpeed;
    
    // has two gears at edges
    private Vector3 startPos;
    private Vector3 endPos;

    private bool isMoving = false;
    private Vector2 speed;

    private void Update()
    {
        var was = PositionWS;
        PositionWS.y += 1;
        foreach (var actor in Level.AllActors)
        {
            if (!isMoving && CollideCheck(actor))
            {
                // move platform
            }
        }
        PositionWS = was;
        
        //when moving .. start slow and get fast
    }
}


