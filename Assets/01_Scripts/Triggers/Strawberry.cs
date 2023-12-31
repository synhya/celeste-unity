
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class Strawberry : Trigger
{
    [SerializeField][Min(0f)] private float shrinkDistanceThreshold = 6;
    [SerializeField][Min(0f)] private float speed = 15;
    
    private Animator anim;

    private const string StAnimBlink = "Strawberry_Blink";
    private const string StAnimShrink = "Strawberry_Shrink";
    private readonly int HashBlink = Animator.StringToHash(StAnimBlink);
    private readonly int HashShrink = Animator.StringToHash(StAnimShrink);
    
    private bool isBlinking = false;
    private bool onBlink = false;
    private bool isFollowing = false;
    private float timer;
    private Entity target;
    
    // strawberries should have id
    [SerializeField] private int id;

    private bool isTaken;

    private TweenerCore<Vector3, Vector3, VectorOptions> lerpTween;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        
        // keep moving up and down with tween
        lerpTween = transform.DOLocalMoveY(6, 1.4f, true).SetRelative()
            .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public override void OnEnter(Entity other)
    {
        if (other is Player)
        {
            // player add score
            (other as Player).OnAddStrawberry(id);
            
            // play blink and follow player
            anim.Play(StAnimBlink);
            target = other;
            onBlink = true;

            isTaken = true;
            lerpTween.Kill();
            transform.SetParent(Level.transform);
        }
    }
    public override void OnStay(Entity other)
    {
    }
    public override void OnLeave(Entity other)
    {
    }

    private void Update()
    {
        if (onBlink)
        {
            onBlink = false;
            isBlinking = true;
            timer = 0.2f;
        }
        
        if (timer > 0f)
            timer -= Time.deltaTime;
        else if (isBlinking)
        {
            isBlinking = false;
            isFollowing = true;
        }

        if (isFollowing)
        {
            var t = transform;
            
            if (Vector2.Distance(target.CenterWS, t.position) < shrinkDistanceThreshold)
            {
                isFollowing = false;
                anim.Play(StAnimShrink);
            }
            else if (!Game.IsPaused)
            {
                // follow player 
                t.position = Vector3.MoveTowards(t.position, target.CenterWS,
                    speed * Time.deltaTime);
            }
        }
    }

    private void LateUpdate()
    {
        if (Collideable && isTaken)
            Collideable = false;
    }

    public void OnShrinkAnimEnd()
    {
        // Destroy 
        Destroy(gameObject, 0.1f);
    }
}


