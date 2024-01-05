using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// manage all the clips here
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager I => instance;
    private static SoundManager instance = null;
    
    private AudioSource bgmSource;
    
    // player move sounds
    public AudioClip[] jumpSnd;
    public AudioClip[] wallJumpSnd;
    public AudioClip[] dashSnd;
    public AudioClip[] snowWalkSnd;
    
    // player death sounds
    
    // gear sounds
    
    // spring sounds
    
    // 
    
    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        bgmSource = GetComponent<AudioSource>();
        bgmSource.Play();
    }
} 
