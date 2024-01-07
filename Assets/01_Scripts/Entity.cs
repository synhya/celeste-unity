using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/// <summary>
/// base class for solid and actor
/// </summary>
[RequireComponent(typeof(AudioSource))]
public abstract class Entity : MonoBehaviour, ISoundable
{
    protected Level Level => Level.Current;

    [Header("Entity Settings")]
    [SerializeField] private Transform targetTransform;
    public Vector2Int HitBoxBottomLeftOffset;
    [SerializeField] private Vector2Int hitBoxSize;
    [HideInInspector] public RectInt HitBoxWS;
    
    [HideInInspector] public Vector2Int PreviousPos;
    private Vector2Int positionWS;
    
    
    [HideInInspector] public Vector2 Speed;
    protected Vector2 Remainder;
    protected bool Collideable = true;
    
    protected const int TileSize = 8;
    
    #region Properties
    public Vector2Int PositionWS
    {
        get => positionWS;
        set 
        {
            positionWS = value;
            HitBoxWS.position = value + HitBoxBottomLeftOffset;
        }
    }
    public Vector2Int HitboxSize
    {
        get => hitBoxSize;
        set 
        {
            hitBoxSize = value;
            HitBoxWS.size = value;
        }
    }
    
    #endregion

    #region Pos Shortcut

    public Vector2 CenterWS => HitBoxWS.center;
    public Vector2 CenterRightWS => CenterWS + Vector2.right * (hitBoxSize.x * 0.5f);
    public Vector2 CenterLeftWS => CenterWS - Vector2.right * (hitBoxSize.x * 0.5f);
    public Vector2 BottomRightWS => new Vector2(HitBoxWS.xMax, HitBoxWS.yMin);
    public Vector2 BottomLeftWS => HitBoxWS.position;  
    
    public int RightWS => HitBoxWS.xMax;
    public int LeftWS => HitBoxWS.xMin;
    public int DownWS => HitBoxWS.yMin;
    public int UpWS => HitBoxWS.yMax;

    #endregion
    
    protected virtual void Awake()
    {
        SndSource = GetComponent<AudioSource>();
        SndSource.playOnAwake = false;
        
        if (!targetTransform)
            targetTransform = transform;
        
        PositionWS = Vector2Int.RoundToInt(targetTransform.position);
        HitBoxWS = new RectInt(PositionWS + HitBoxBottomLeftOffset, hitBoxSize);
    }
    
    protected virtual void Start()
    {
    }

    protected void UpdatePosition()
    {
        if (PositionWS != PreviousPos)
        {
            targetTransform.position = (Vector3Int)PositionWS;
        }
        
        PreviousPos = PositionWS;
    }
    
    protected bool CollideCheck(Entity other)
    {
        if (other != this && other.Collideable
            && HitBoxWS.Overlaps(other.HitBoxWS))
        {
            return true;
        }
        return false;
    }

    // sounds
    protected SoundClipManager Clips => SoundClipManager.I;
    protected AudioSource SndSource;
    public void PlaySound(AudioClip clip, float pitch = 1f, float startRatio = 0f)
    {
        SndSource.Stop();
        SndSource.pitch = pitch;
        SndSource.time = startRatio * clip.length;
        SndSource.clip = clip;
        SndSource.Play();
    }

    public void StopSound() => SndSource.Stop();
}
