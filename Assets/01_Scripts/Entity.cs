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
    public Vector2Int HitboxBottomLeftOffset;
    public Vector2Int HitboxSize;

    [HideInInspector] public Vector2Int PositionWS;
    [HideInInspector] public Vector2Int PreviousPos;
    
    private RectInt varBox;
    public RectInt HitBoxWS
    {
        get {
            varBox.position = PositionWS + HitboxBottomLeftOffset;
            varBox.size = HitboxSize;
            return varBox;
        }
    }

    #region Pos Shortcut

    public Vector2 CenterWS => PositionWS + Vector2.up * (HitboxSize.y * 0.5f);
    public Vector2 CenterRightWS => CenterWS + Vector2.right * (HitboxSize.x * 0.5f);
    public Vector2 CenterLeftWS => CenterWS - Vector2.right * (HitboxSize.x * 0.5f);
    public Vector2 BottonRightWS => PositionWS + Vector2.right * (HitboxSize.x * 0.5f);
    public Vector2 BottonLeftWS => PositionWS - Vector2.right * (HitboxSize.x * 0.5f);    

    #endregion

    public int RightWS => HitBoxWS.xMax;
    public int LeftWS => HitBoxWS.xMin;
    public int DownWS => HitBoxWS.yMin;
    public int UpWS => HitBoxWS.yMax;

    public const int TileSize = 8;


    public Vector2 Speed;
    protected Vector2 Remainder;


    protected virtual void Start()
    {
        PositionWS = Vector2Int.RoundToInt(transform.position);
    }

    protected void UpdatePosition()
    {
        if (PositionWS != PreviousPos)
        {
            transform.position = (Vector3Int)PositionWS;
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
