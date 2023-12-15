
using System;
using UnityEngine;

/// <summary>
/// all the objects that will interact in the scene
/// </summary>
public class Entity : MonoBehaviour
{
    // scene is pixel perfect so 
    // every entity will have pixel array in which they belong to
    public Vector2Int[] GridPositions;

    protected virtual void Start()
    {
        // set grid position
        
    }
}


