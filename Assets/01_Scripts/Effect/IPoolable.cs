
using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable
{
    /// <summary>
    /// init is called instead of start method
    /// </summary>
    /// <param name="pool"></param>
    public void Init(IObjectPool<GameObject> pool);
}


