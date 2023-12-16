
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// base static tile map where player stands on
/// </summary>
public class SolidTilemap : Entity
{
    private TilemapCollider2D tilemapCollider2D;


    public override void Init()
    {
        base.Init();
        if(!TryGetComponent(out tilemapCollider2D))
            Debug.LogError("no collider attached");
    }
}


