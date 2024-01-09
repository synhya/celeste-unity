
// base class for monsters

using UnityEngine;
using UnityEngine.Serialization;

public class Monster : Actor
{
    // monsters should have info of where player HitBox is
    private Player player;
    
    public float PatrolMaxSpeedX = 160f;
    public float ChaseMaxSpeedX = 220f;
    public float Accel = 400f;
    public int PatrolRangeX;
    public int NoticeExtentX = 3;
    
    private StateMachine sm;
    public const int StatePatrol = 0;
    public const int StateRest = 1;
    public const int StateChase = 2;
    public const int StateAttack = 3;
    public const int StateDead = 4;

    private Animator anim;
    
    // vars
    private int moveDirX = 1;
    private int patrolMaxX;
    private int patrolMinX;
    private bool arrived = false;

    private float timer;

    #region Properties

    private RectInt NoticeRect
    {
        get {
            var noticeRect = HitBoxWS;
            noticeRect.x = PositionWS.x - NoticeExtentX;
            noticeRect.width = NoticeExtentX * 2;
            return noticeRect;
        }
    }

    #endregion
    
    protected override void Awake()
    {
        base.Awake();

        sm = new StateMachine(4);
        sm.SetCallbacks(StatePatrol, PatrolUpdate, PatrolBegin, null);
        sm.SetCallbacks(StateChase, ChaseUpdate, ChaseBegin, ChaseEnd);
        sm.SetCallbacks(StateRest, RestUpdate, RestBegin, null);
        sm.SetCallbacks(StateDead, DeadUpdate, DeadBegin, null);

        patrolMaxX = PositionWS.x + PatrolRangeX;
        patrolMinX = PositionWS.x - PatrolRangeX;

        anim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        
        player = Game.MainPlayer;
    }

    protected override void Update()
    {
        sm.Update();
        
        // any time player hits monster 
        // if it is not dead kill it
        
        UpdatePosition();
    }

    #region State Patrol

    private void PatrolBegin()
    {
        anim.Play("Skeleton_Walk");
    }

    private int PatrolUpdate()
    {
        // check if nearby player exists
        if (NoticeRect.Overlaps(player.HitBoxWS))
            return StateChase;
        
        // and move every frame
        Speed.x = Mathf.MoveTowards(Speed.x, PatrolMaxSpeedX * moveDirX, Time.deltaTime * Accel);

        var moveAmount = Speed.x * Time.deltaTime;

        var leftDist = moveDirX == 1 ? patrolMaxX - PositionWS.x : 
            PositionWS.x - patrolMinX;
        
        if (Mathf.Abs(moveAmount) > leftDist)
        {
            moveAmount = leftDist * moveDirX;
            arrived = true;
        }
        
        MoveH(moveAmount, null);

        if (arrived)
        {
            arrived = false;
            moveDirX *= -1;
            return StateRest;
        }

        return StatePatrol;
    }

    #endregion

    #region State Rest

    private void RestBegin()
    {
        timer = Random.Range(1f, 3f);
        anim.Play("Skeleton_Idle");
    }

    private int RestUpdate()
    {
        // play idle here , don't move
        if (timer > 0f) timer -= Time.deltaTime;
        else return StatePatrol;

        return StateRest;
    }

    #endregion
    
    private void ChaseBegin()
    {
        anim.Play("Skeleton_Walk");
        anim.speed = 1.5f;
    }

    private int ChaseUpdate()
    {
        // follow player until too far away

        return StateChase;
    }
    
    private void ChaseEnd()
    {
        anim.speed = 1f;
    }

    #region Dead State

    private void DeadBegin()
    {
        anim.Play("Skeleton_Death");
        timer = 5f;
    }

    public int DeadUpdate()
    {
        if (timer > 0f) timer -= Time.deltaTime;
        else
        {
            anim.speed = -1;
            anim.Play("Skeleton_Death");
        }

        return StateDead;
    }

    #endregion
}


