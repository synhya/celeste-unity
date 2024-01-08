using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct AudioData
{
    public AudioClip clip;
    [Range(0, 1)] public float volume;
    [Range(0.1f, 5f)] public float pitch;
}

/// <summary>
/// manage all the clips here
/// </summary>
public class SoundDataManager : MonoBehaviour
{
    public static SoundDataManager I => instance;
    private static SoundDataManager instance = null;

    [Header("Common")]
    public AudioData menuWindSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData menuClickSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData menuStartLevelSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData levelBgmSndData = new AudioData{pitch = 1f, volume = 0.5f};
    
    // player move sounds
    [Header("Player Move")]
    public AudioData jumpSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData wallJumpSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData dashSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData snowWalkSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData metalWalkSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData stoneWalkSndData = new AudioData{pitch = 1f, volume = 0.5f};
    
    // player death sounds
    [Header("Player Death")]
    public AudioData playerDeathSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData playerReviveSndData = new AudioData{pitch = 1f, volume = 0.5f};
    
    // platform snds
    [Header("Platform")]
    public AudioData gearBellSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData gearSpinSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData gearSpinBackSndData = new AudioData{pitch = 1f, volume = 0.5f};

    public AudioData brickShakeSndData = new AudioData{pitch = 1f, volume = 0.5f};
    
    // trigger sounds
    [Header("Triggers")]
    public AudioData springSndData = new AudioData{pitch = 1f, volume = 0.5f};
    public AudioData strawberrySndData = new AudioData{pitch = 1f, volume = 0.5f};
    
    void Awake() 
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
    }
    
    public void Play(AudioSource source, AudioData data)
    {
        source.Stop();
        source.pitch = data.pitch;
        source.volume = data.volume;
        // source.time = startRatio * clip.length;
        source.clip = data.clip;
        source.Play();
    }
} 
