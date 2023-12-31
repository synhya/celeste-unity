
using UnityEngine;
using static UnityEngine.Mathf;

public class GearPlatform : Solid
{
    public int GearOffset = 8;
    
    [SerializeField] private float maxForwardSpeed = 180;
    [SerializeField] private float maxRewindSpeed = 30;
    [SerializeField] private float accel = 1000;
    
    // has two gears at edges
    private Vector2Int[] edges;
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;

    private bool isMovingForward = false;
    private bool isMovingBackward = false;
    private Vector2 dir;
    private Vector2Int leftDist;

    private Animator[] anims;
    
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
        if (!isMovingForward && !isMovingBackward && IsActorRiding())
        {
            isMovingForward = true;
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
                    // reset remainder and speed 
                    isMovingForward = false;
                    isMovingBackward = true;
                    dir = -dir;
                    Speed = Vector2.zero;
                }
                else
                {
                    Speed = Vector2.MoveTowards(Speed, dir * maxForwardSpeed, accel * Time.deltaTime);
                }
            }
            else if (isMovingBackward)
            {
                leftDist = edges[0] - PositionWS;
                if (leftDist.magnitude == 0)
                {
                    // reset remainder and speed 
                    isMovingBackward = false;
                    dir = -dir;
                    Speed = Vector2.zero;
                }
                else
                {
                    Speed = Vector2.MoveTowards(Speed, dir * maxRewindSpeed, accel * Time.deltaTime);
                }
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
    
    private bool IsActorRiding()
    {
        var was = PositionWS;
        var ret = false;
        PositionWS.y += 1;
        foreach (var actor in Level.AllActors)
        {
            if (OverlapCheck(actor))
            {
                ret = true;
                break;
            }
        }
        PositionWS = was;

        return ret;
    }
}


