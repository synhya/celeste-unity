using System.Collections.Generic;
using UnityEngine;

public class RoomMananger : MonoBehaviour
{
    public static RoomMananger Room =>
        (instance ??= (new GameObject("Manager")).AddComponent<RoomMananger>());
    private static RoomMananger instance = null;


    public List<Solid> Solids;
    public List<Actor> Actors;
    
    private void Awake()
    {
        if (instance != this)
        {
            if (instance == null) instance = this;
            else Destroy(this);
        }
        
        DontDestroyOnLoad(this);

        Solids = new List<Solid>();
    }
}


