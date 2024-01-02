using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/// <summary>
/// base class for solid and actor
/// </summary>
public abstract class Entity : MonoBehaviour
{
    protected Level Level => Game.I.CurrentLevel;
    
    protected bool Collideable = true;

    [Header("Entity Settings")]
    [SerializeField] private Transform targetTransform;
    public Vector2Int HitboxBottomLeftOffset;
    [FormerlySerializedAs("HitboxSize")]
    [SerializeField] private Vector2Int hitboxSize;
    private Vector2Int positionWS;
    
    [HideInInspector] public Vector2Int PreviousPos;
    [HideInInspector] public RectInt HitBoxWS;
    
    #region Properties
    public Vector2Int PositionWS
    {
        get => positionWS;
        set 
        {
            HitBoxWS.position = PositionWS + HitboxBottomLeftOffset;
            positionWS = value;
        }
    }
    public Vector2Int HitboxSize
    {
        get => hitboxSize;
        set 
        {
            HitBoxWS.size = hitboxSize;
            hitboxSize = value;
        }
    }
    
    #endregion

    #region Pos Shortcut

    public Vector2 CenterWS => HitBoxWS.center;
    public Vector2 CenterRightWS => CenterWS + Vector2.right * (hitboxSize.x * 0.5f);
    public Vector2 CenterLeftWS => CenterWS - Vector2.right * (hitboxSize.x * 0.5f);
    public Vector2 BottomRightWS => new Vector2(HitBoxWS.xMax, HitBoxWS.yMin);
    public Vector2 BottomLeftWS => HitBoxWS.position;  
    
    public int RightWS => HitBoxWS.xMax;
    public int LeftWS => HitBoxWS.xMin;
    public int DownWS => HitBoxWS.yMin;
    public int UpWS => HitBoxWS.yMax;

    #endregion

    public const int TileSize = 8;


    [HideInInspector] public Vector2 Speed;
    protected Vector2 Remainder;
    
    // left to implement.
    public virtual void Added(Level level)
    {
    }
    
    protected virtual void Awake()
    {
        targetTransform ??= transform;
    }
    
    protected virtual void Start()
    {
        PositionWS = Vector2Int.RoundToInt(targetTransform.position);
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
}
