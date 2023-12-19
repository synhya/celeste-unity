
using System;
using System.Net.Http.Headers;
using UnityEngine;
using static UnityEngine.Mathf;

public class Player : Actor
{
    [Header("Player Settings")]
    [SerializeField] private float maxrun = 1;
    [SerializeField] private float accel = 0.6f;
    [SerializeField] private float accelOnAir = 0.4f;
    [SerializeField] private float deccel = 0.15f;
    
    
    private bool onGround;
    
    
    private void Update()
    {
        var input = Input.GetAxisRaw("Horizontal");
        
        // move
        

        Speed.x += input * accel * Time.deltaTime;
        if (Abs(Speed.x) > maxrun)
            Speed.x = Sign(Speed.x) * Max(Abs(Speed.x) - deccel, maxrun);
        
        // get player tile position
        // then check bottom is solid
        
        
        MoveX(Speed.x, null);
        MoveY(Speed.y, null);


        UpdatePosition();
    }
}


