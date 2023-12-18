
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


    /// <summary>
    /// moving object (not solid)
    /// </summary>
    class platform : ClassicObject
    {
        public float dir;
        private float last;

        public override void init(Classic g, Emulator e)
        {
            base.init(g, e);
            x -= 4;
            solids = false;
            hitbox.width = 16;
            last = x;
        }

        public override void update()
        {
            spd.x = dir * 0.65f;
        }
    }

    class fall_floor : ClassicObject
    {
        
    }

    class fake_wall : ClassicObject
    {
        
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
        

        /// <param name="ox">difference_x</param>
        /// <param name="oy">difference_y</param>
        /// <returns></returns>
        public bool is_solid(int ox, int oy)
        {
            if (oy > 0 && !check<platform>(ox, 0) && check<platform>(ox, oy))
                return true;
            return G.solid_at(x + hitbox.x + ox, y + hitbox.y + oy, hitbox.width, hitbox.height) ||
                   check<fall_floor>(ox, oy) ||
                   check<fake_wall>(ox, oy);
        }

        /// <summary>
        /// how about non rectangluar object such as platform
        /// </summary>
        public T collide<T>(int ox, int oy) where T : ClassicObject
        {
            var type = typeof(T);
            foreach (var other in G.objects)
            {
                if (other != null && other.GetType() == type &&
                    other != this && other.collideable &&
                    other.x + other.hitbox.x + other.hitbox.width > x + hitbox.x + ox &&
                    other.y + other.hitbox.y + other.hitbox.height > y + hitbox.y + oy &&
                    other.x + other.hitbox.x < x + hitbox.x + ox &&
                    other.y + other.hitbox.y < y + hitbox.y + oy)
                    return other as T;
            }
            
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

    #region util

    private float appr(float val, float target, float amount)
    {
        return (val > target ? Mathf.Max(val - amount, target) : 
            Mathf.Min(val + amount, target));
    }

    private bool solid_at(float x, float y, float w, float h)
    {
        return tile_flag_at(x, y, w, h, 0);
    }

    private bool tile_flag_at(float x, float y, float w, float h, int flag)
    {
        

        return false;
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


