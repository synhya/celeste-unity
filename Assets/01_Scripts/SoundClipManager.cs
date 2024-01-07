using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// manage all the clips here
/// </summary>
public class SoundClipManager : MonoBehaviour
{
    public static SoundClipManager I => instance;
    private static SoundClipManager instance = null;
    
    [Header("Common")]
    public AudioClip menuWindSnd;
    public AudioClip menuClickSnd;
    public AudioClip menuStartLevelSnd;
    public AudioClip levelBgmSnd;
    
    // player move sounds
    [Header("Player Move")]
    public AudioClip jumpSnd;
    public AudioClip wallJumpSnd;
    public AudioClip dashSnd;
    public AudioClip snowWalkSnd;
    public AudioClip metalWalkSnd;
    public AudioClip stoneWalkSnd;
    
    // player death sounds
    [Header("Player Death")]
    public AudioClip playerDeathSnd;
    public AudioClip playerReviveSnd;
    
    // platform snds
    [Header("Platform")]
    public AudioClip gearBellSnd;
    [FormerlySerializedAs("gearForwardSnd")]
    public AudioClip gearSpinSnd;
    
    [FormerlySerializedAs("brickFadeSnd")]
    public AudioClip brickShakeSnd;
    
    // trigger sounds
    [Header("Triggers")]
    public AudioClip springSnd;
    public AudioClip strawberrySnd;
    
    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
    }
} 
