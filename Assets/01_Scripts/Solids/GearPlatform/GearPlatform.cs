
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.Mathf;

public class GearPlatform : Solid
{
    public int GearOffset = 8;

    [Header("Traffic Light Settings")]
    [SerializeField] private SpriteRenderer lightSR;
    [SerializeField] private Sprite[] lightSprites;
    [SerializeField] private Texture2D[] hdrTex;
    
    [Header("Movement Settings")]
    [SerializeField] private float maxForwardSpeed = 180;
    [SerializeField] private float maxRewindSpeed = 30;
    [SerializeField] private float accel = 1000;
    [SerializeField] private float StopTime = 0.3f;
    [SerializeField] private float KeepSpeedTime = 0.1f;
    
    // has two gears at edges
    private Vector2Int[] edges;
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;

    private bool isMovingForward = false;
    private bool isMovingBackward = false;
    private bool stoppedAtEdge = false;
    private Vector2 dir;
    private Vector2Int leftDist;

    private Animator[] anims;
    private SpriteRenderer blockSR;
    public AudioClip[] forwardSnd;
    public AudioClip[] backwardSnd;

    private float timer;
    
    protected override void Start()
    {
        base.Start();
        
        var s = Vector2Int.RoundToInt(start.position);
        s.y -= GearOffset;
        var e = Vector2Int.RoundToInt(end.position);
        e.y -= GearOffset;
        
        // set edges
        edges = new[]{ s, e };
        PositionWS = edges[0];
        UpdatePosition();
        
        // set anims
        var a1 = start.GetComponent<Animator>();
        var a2 = end.GetComponent<Animator>();
        
        anims = new[]{ a1, a2 };
        
        // get direction
        dir =  edges[1] - PositionWS;
        dir = dir.normalized;
    }
    

    private void Update()
    {
        if (timer > 0f)
        {
            if (timer >= StopTime - KeepSpeedTime)
                Speed = Vector2.MoveTowards(Speed, Vector2.zero, accel * 0.45f * Time.deltaTime);
            else
                Speed = Vector2.zero;
            timer -= Time.deltaTime;
        }
        else if (!isMovingForward && !isMovingBackward && !stoppedAtEdge && HasActorRiding())
        {
            isMovingForward = true;
            lightSR.sprite = lightSprites[0];
            lightSR.material.SetTexture("_EmissionTex", hdrTex[0]);
            
            PlaySound(Clips.gearForwardSnd, 2f, 0.3f);
        }
        else if (stoppedAtEdge)
        {
            stoppedAtEdge = false;
            isMovingBackward = true;

            PlaySound(Clips.gearBackwardSnd, 1f, 0.2f);
        }
        
        if (isMovingForward || isMovingBackward)
        {
            // should animate gear
            anims[0].enabled = true;
            anims[1].enabled = true;
            anims[0].Play("Gear_Play");
            anims[1].Play("Gear_Play");
            
            if (isMovingForward)
            {
                leftDist = edges[1] - PositionWS;
                if (leftDist.magnitude == 0)
                {
                    isMovingForward = false;
                    dir = -dir;
                    stoppedAtEdge = true;
                    timer = StopTime;
                    
                    anims[0].speed = 0.6f;
                    anims[1].speed = 0.6f;
                    lightSR.sprite = lightSprites[1];
                    lightSR.material.SetTexture("_EmissionTex", hdrTex[1]);
                    
                    StopSound();
                }
                else
                    Speed = Vector2.MoveTowards(Speed, dir * maxForwardSpeed, accel * Time.deltaTime);
            }
            else if (isMovingBackward)
            {
                leftDist = edges[0] - PositionWS;
                if (leftDist.magnitude == 0)
                {
                    isMovingBackward = false;
                    dir = -dir;
                    Speed = Vector2.zero;
                    anims[0].speed = 1f;
                    anims[1].speed = 1f;
                    lightSR.sprite = lightSprites[2];
                    lightSR.material.SetTexture("_EmissionTex", hdrTex[2]);
                    
                    StopSound();
                }
                else
                    Speed = Vector2.MoveTowards(Speed, dir * maxRewindSpeed, accel * Time.deltaTime);
            }   
            
            var moveAmount = Speed * Time.deltaTime;
            if (Abs(moveAmount.x) > Abs(leftDist.x))
                moveAmount.x = leftDist.x;
            if (Abs(moveAmount.y) > Abs(leftDist.y))
                moveAmount.y = leftDist.y;
        
            Move(moveAmount.x, moveAmount.y);
        }
        else
        {
            anims[0].enabled = false;
            anims[1].enabled = false;
        }
        UpdatePosition();
    }
}


