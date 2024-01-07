
using System;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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

    [Header("Time Settings")]
    [SerializeField] private float startDelayTime = 0.2f;
    [SerializeField] private float restTime = 0.3f;
    
    // has two gears at edges
    private Vector2Int[] edges;
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;

    private StateMachine sm;
    private const int StWait = 1;
    private const int StForward = 2;
    private const int StArrive = 2;
    private const int StBackward = 2;
    
    private Vector2 forwardDirection;
    private Vector2 currentDirection;
    private Vector2Int leftDist;

    private Animator[] anims;
    private SpriteRenderer blockSR;

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
        anims[0].Play("Gear_Play");
        anims[1].Play("Gear_Play");
        anims[0].enabled = false;
        anims[1].enabled = false;
        
        // get direction
        forwardDirection =  edges[1] - PositionWS;
        forwardDirection = forwardDirection.normalized;
        
        // set states
        sm = new StateMachine(4);
        sm.SetCallbacks(StWait, WaitUpdate, null, null);
        sm.SetCallbacks(StForward, ForwardUpdate, ForwardBegin, ForwardEnd);
        sm.SetCallbacks(StArrive, ArriveUpdate, ArriveBegin, ArriveEnd);
        sm.SetCallbacks(StBackward, BackwardUpdate, BackwardBegin, BackwardEnd);
        
        sm.State = StWait;
    }
    
    private void Update()
    {
        sm.Update();   
    }

    private int WaitUpdate()
    {
        if (timer <= 0f && HasActorRiding())
        {
            PlaySound(Clips.gearBellSnd);
            timer = startDelayTime;
        }
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
                return StForward;
        }
        
        return StWait;
    }

    #region State Forward

    void ForwardBegin()
    {
        currentDirection = forwardDirection;
        
        lightSR.sprite = lightSprites[0];
        lightSR.material.SetTexture("_EmissionTex", hdrTex[0]);
        
        anims[0].enabled = true;
        anims[1].enabled = true;
        
        PlaySound(Clips.gearSpinSnd, 2f);
    }

    int ForwardUpdate()
    {
        leftDist = edges[1] - PositionWS;
        if (leftDist.magnitude == 0)
        {
            return StArrive;
        }

        Speed = Vector2.MoveTowards(Speed, currentDirection * maxForwardSpeed, accel * Time.deltaTime);

        var moveAmount = Speed * Time.deltaTime;
        if (Abs(moveAmount.x) > Abs(leftDist.x))
            moveAmount.x = leftDist.x;
        if (Abs(moveAmount.y) > Abs(leftDist.y))
            moveAmount.y = leftDist.y;
    
        Move(moveAmount.x, moveAmount.y);

        UpdatePosition();
        
        return StForward;
    }

    void ForwardEnd()
    {
        anims[0].enabled = false;
        anims[1].enabled = false;
    }

    #endregion

    #region State Arrive

    private void ArriveBegin()
    {
        timer = restTime;
        StopSound();
    }

    private int ArriveUpdate()
    {
        if (timer > 0f)
        {
            Speed = Vector2.MoveTowards(Speed, Vector2.zero, accel * 0.3f * Time.deltaTime);
            
            timer -= Time.deltaTime;
        }
        else
            return StBackward;
        
        return StArrive;
    }
    
    private void ArriveEnd() => Speed = Vector2.zero;

    #endregion

    #region State Backward

    private void BackwardBegin()
    {
        currentDirection = -forwardDirection;
        
        lightSR.sprite = lightSprites[1];
        lightSR.material.SetTexture("_EmissionTex", hdrTex[1]);
        
        anims[0].speed = 0.6f;
        anims[1].speed = 0.6f;
        anims[0].enabled = true;
        anims[1].enabled = true;
        
        PlaySound(Clips.gearSpinSnd, 1f);
    }

    private int BackwardUpdate()
    {
        leftDist = edges[0] - PositionWS;
        if (leftDist.magnitude == 0)
        {
            return StWait;
        }
        
        Speed = Vector2.MoveTowards(Speed, currentDirection * maxRewindSpeed, accel * Time.deltaTime);
        
        var moveAmount = Speed * Time.deltaTime;
        if (Abs(moveAmount.x) > Abs(leftDist.x))
            moveAmount.x = leftDist.x;
        if (Abs(moveAmount.y) > Abs(leftDist.y))
            moveAmount.y = leftDist.y;
    
        Move(moveAmount.x, moveAmount.y);

        UpdatePosition();
        
        return StBackward;
    }

    private void BackwardEnd()
    {
        anims[0].speed = 1f;
        anims[1].speed = 1f;
        anims[0].enabled = false;
        anims[1].enabled = false;
    }

    #endregion
}


