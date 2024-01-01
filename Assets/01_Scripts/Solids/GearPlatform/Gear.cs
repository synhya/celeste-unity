
using UnityEngine;
using UnityEngine.Pool;

public class Gear : MonoBehaviour, IPoolable
{

    private IObjectPool<Gear> pool;
    private Animator anim;
    
    public void Init<T>(IObjectPool<T> pool) where T : MonoBehaviour
    {
        this.pool = pool as IObjectPool<Gear>;
        anim = GetComponent<Animator>();
    }

    public void Play(float speed = 1f)
    {
        anim.Play("Gear_Play");
        anim.speed = speed;
    }

    public void Release()
    {
        pool.Release(this);
    }
}


