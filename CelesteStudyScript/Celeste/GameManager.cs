using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;


/// <summary>
/// class Classic
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager G =>
        (instance ??= (new GameObject("Manager")).AddComponent<GameManager>());
    private static GameManager instance = null;

    /// <summary>
    /// entities in current room
    /// </summary>
    public List<Entity> Entities;

    public TilemapCollider2D spikeCollider;
    
    public bool PausePlayer => pausePlayer;
    private bool pausePlayer = false;
    

    private void Awake()
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
        
        // init 
        Entities = new List<Entity>();
    }


    public T InitEntity<T>(T entity, float x, float y) 
        where T : Entity
    {
        Entities.Add(entity);
        entity.Position.x = (int)x;
        entity.Position.y = (int)y;
        entity.Init(G);

        return entity;
    }

    public void DestroyEntity<T>(T entity) where T : Entity
    {
        var index = Entities.IndexOf(entity);
        if (index >= 0)
            Entities[index] = null; // remove from room.
        Destroy(entity.gameObject);
    }

    public void KillPlayer(Player player)
    {
        DestroyEntity(player);
    }
    
    public bool SpikesAt(Collider2D col, Vector2 spd)
    {
        return col.IsTouching(spikeCollider);
    }
    
    
}


