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
    public bool Collideable = true;
    
    protected const int TileSize = 8;
    
    #region Properties
    
    public float X
    {
        get => positionWS.x;
        set {
            var pos = PositionWS;

            Remainder.x += value;
            int move = Mathf.RoundToInt(Remainder.x);
            Remainder.x -= move;
            
            pos.x = move;
            PositionWS = pos;
        }
    }
    
    public float Y
    {
        get => positionWS.y;
        set {
            var pos = PositionWS;
            
            Remainder.y += value;
            int move = Mathf.RoundToInt(Remainder.y);
            Remainder.y -= move;
            
            pos.y = move;
            PositionWS = pos;
        }
    }
    
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
        BaseSoundSource = GetComponent<AudioSource>();
        BaseSoundSource.playOnAwake = false;
        
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
    protected SoundDataManager Sound => SoundDataManager.I;
    protected AudioSource BaseSoundSource;
    public void PlaySound(AudioData data)
    {
        BaseSoundSource.Stop();
        BaseSoundSource.pitch = data.pitch;
        // BaseSoundSource.time = startRatio * data.length;
        BaseSoundSource.volume = data.volume;
        BaseSoundSource.clip = data.clip;
        BaseSoundSource.Play();
    }

    public void StopSound() => BaseSoundSource.Stop();
}
