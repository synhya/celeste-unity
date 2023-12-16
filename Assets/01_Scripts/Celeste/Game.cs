using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// class Classic
/// </summary>
public class Game : MonoBehaviour
{
    public static Game G =>
        (instance ??= (new GameObject("Manager")).AddComponent<Game>());
    private static Game instance = null;
    
    /// <summary>
    /// entities in current room
    /// </summary>
    public static List<Entity> Entities => G.entities;
    private List<Entity> entities;

    private void Awake()
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);
        
        // init 
        entities = new List<Entity>();
    }


    private T InitEntity<T>(T entity, float x, float y, int? tile = null) 
        where T : Entity
    {
        entities.Add(entity);
        if (tile.HasValue)
            entity.SpriteIdx = tile.Value;
        entity.Position.x = (int)x;
        entity.Position.y = (int)y;
        entity.Init();

        return entity;
    }

    private void DestroyEntity<T>(T entity) where T : Entity
    {
        var index = entities.IndexOf(entity);
        if (index >= 0)
            entities[index] = null; // remove from room.
        Destroy(entity.gameObject);
    }
}


