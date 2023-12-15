
using System.Collections.Generic;
using UnityEngine;

public class Classic
{
    public Emulator E;

    private List<ClassicObject> objects;
    private Vector2 room;
    
    private HashSet<int> got_fruit;
    private int frames;
    private int deaths;
    private int max_djump;
    private bool start_game;
    private int start_game_flash;
    private bool has_dashed;
    private bool has_key;

    private void title_screen()
    {
        got_fruit = new HashSet<int>();
        frames = 0;
        deaths = 0;
        max_djump = 1; // dashJump
        start_game = false;
        start_game_flash = 0;
        E.music(40, 0, 7);
        load_room(7, 3);
    }

    #region objects



    class platform : ClassicObject
    {
        public float dir;
    }

    #endregion
    

    #region object functions

    /// <summary>
    /// entity
    /// </summary>
    public class ClassicObject
    {
        public Classic G;

        public Emulator E;

        public int type;

        public bool collideable = true;
        public bool solids = true;
        public float spr;

        public bool flipX;
        public bool flipY;

        public float x;
        public float y;

        public Rect hitbox = new Rect(0, 0, 8, 8);
        public Vector2 spd = new Vector2(0, 0);
        public Vector2 rem = new Vector2(0, 0); // remainder

        public virtual void init(Classic g, Emulator e)
        {
            G = g;
            E = e;
        }

        public virtual void update()
        {
        
        }

        public bool is_solid(int ox, int oy)
        {
            return true;
        }

        public T collide<T>(int ox, int oy) where T : ClassicObject
        {

            return null;
        }

        public bool check<T>(int ox, int oy) where T : ClassicObject
        {
            return collide<T>(ox, oy) != null;
        }

        public void move(float ox, float oy)
        {
            
        }

        public void move_x(int amount, int start)
        {
            
        }
        public void move_y(int amount)
        {
            
        }
    }
    
    private T init_object<T>(T obj, float x, float y, int? tile = null) where T : ClassicObject
    {

        return null;
    }

    #endregion


    #region room functions


    public void load_room(int x, int y)
    {
        has_dashed = false;
        has_key = false;
        
        // remove existing objects
        for (int i = 0; i < objects.Count; i++)
            objects[i] = null;
        
        // current room
        room.x = x;
        room.y = y;
        
        // entities
        for (int tx = 0; tx <= 15; tx++)
        {
            for (int ty = 0; ty <= 15; ty++)
            {
                var tile = E.mget(tx, ty);
                if (tile == 11)
                    init_object(new platform(), tx * 8, ty * 8).dir = -1;
            }
        }
        
    }

    #endregion
}



public class Emulator : MonoBehaviour
{
    public AudioClip music_pico8_area1;
    public AudioClip music_pico8_area2;
    public AudioClip music_pico8_area3;
    public AudioClip music_pico8_area4;

    private AudioSource audioSource;

    public static int[,] tileMap;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        tileMap = new int[16, 16]; // size of screen tile #
    }

    // get from map
    public int mget(int x, int y)
    {
        // get tile information of the coordinate
        return tileMap[x, y];
    }
    
    public void music(int index, int fade, int mask)
    {
        if (index == -1)
        {
            audioSource.clip = null;
        }
        else if (index == 0)
        {
            audioSource.clip = music_pico8_area1;
        }
        else if (index == 10)
        {
            audioSource.clip = music_pico8_area2;
        }
        else if (index == 20)
        {
            audioSource.clip = music_pico8_area3;
        }
        else if (index == 30)
        {
            audioSource.clip = music_pico8_area4;
        }
    }
}


