
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FadePlatform : Solid
{
    [SerializeField] private float FadeOutTime = 0.8f;
    [SerializeField][Range(0, 1)] private float ShakeFadeRatio = 0.6f;
    [SerializeField] private float ReSpawnTime = 1.5f;
    [SerializeField] private float FadeInTime = 0.3f;
    
    private bool isFadingOut = false;
    private bool isFadingIn = false;
    private float timer;

    private Animator anim;

    [SerializeField] private Sprite[] blockSprites;

    private List<Transform> blockTs;
    private List<SpriteRenderer> blockSRs;
    public int Length = 3;

    private List<Sequence> fadeOutSeq;
    private bool isReadyToBeCollideable;


    protected override void Start()
    {
        base.Start();

        anim = GetComponent<Animator>();
        blockTs = new List<Transform>();
        blockSRs = new List<SpriteRenderer>();
        
        // set sprites for blocks
        foreach (Transform block in transform)
        {
            var sr = block.GetComponent<SpriteRenderer>();
            sr.sprite = blockSprites[Random.Range(0, blockSprites.Length)];
            blockTs.Add(block);
            blockSRs.Add(sr);
        }
    }

    private void Update()
    {
        if (!isFadingOut && Collideable && HasActorRiding())
        {
            isFadingOut = true;
            timer = FadeOutTime;
            
            // also start animation.
            for (int i = 0; i < blockTs.Count; i++)
            {
                
                DOTween.Sequence()
                    .Append(blockTs[i].DOShakePosition(FadeOutTime, 1.4f))
                    .Insert(FadeOutTime * ShakeFadeRatio,
                        blockSRs[i].DOFade(0f, FadeOutTime * (1f - ShakeFadeRatio)));
            }
        }
        else if (isFadingOut) 
        {
            if (timer > 0f)
                timer -= Time.deltaTime;
            else
            {
                Collideable = false;
                isFadingOut = false;
                timer = ReSpawnTime;
            }
        }
        else if (!Collideable && !isFadingIn)
        {
            if (timer > 0f)
                timer -= Time.deltaTime;
            else
            {
                isFadingIn = true;
                timer = FadeInTime;
                
                // also start spawn animation. (gradually)
                for (int i = 0; i < blockTs.Count; i++)
                {
                    blockTs[i].transform.localScale = Vector3.one * 0.6f;
                   var seq =  DOTween.Sequence()
                        .Append(blockSRs[i].DOFade(1f, 0.15f))
                        .Append(blockTs[i].DOScale(Vector3.one * 1.15f, FadeInTime - 0.3f))
                        .Append(blockTs[i].DOScale(Vector3.one, 0.15f));
                }
            }
        }
        else if (isFadingIn)
        {
            if (timer > 0f)
                timer -= Time.deltaTime;
            else
            {
                isFadingIn = false;
                isReadyToBeCollideable = true;
            }
        }

        if (isReadyToBeCollideable)
        {
            var doesOverlap = false;
            foreach (var actor in Level.AllActors)
            {
                if (OverlapCheck(actor))
                {
                    doesOverlap = true;
                    break;
                }
            }
            if (!doesOverlap)
            {
                isReadyToBeCollideable = false;
                Collideable = true;
            }
        }
    }
}


