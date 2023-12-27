
using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable
{
    /// <summary>
    /// init is called instead of start method
    /// </summary>
    /// <param name="pool"></param>
    public void Init<T>(IObjectPool<T> pool) where T : MonoBehaviour;
}


