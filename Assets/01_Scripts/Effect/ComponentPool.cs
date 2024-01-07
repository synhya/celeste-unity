using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class ComponentPool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
{
    [SerializeField] private int maxPoolSize = 5;
    [SerializeField] private GameObject prefab;
    public IObjectPool<T> Pool { get; private set; }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Pool = new ObjectPool<T>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
            OnDestroyPoolObject, true, maxPoolSize, maxPoolSize);

        for (int i = 0; i < maxPoolSize; i++)
        {
            var com = CreatePooledItem();
            com.Init(Pool);
            Pool.Release(com);
        }
    }
    
    T CreatePooledItem() {
    {
        return Instantiate(prefab, transform).GetComponent<T>();
    }}

    void OnReturnedToPool(T com)
    {
        com.gameObject.SetActive(false);
    }

    void OnTakeFromPool(T com)
    {
        com.gameObject.SetActive(true);
    }

    void OnDestroyPoolObject(T com)
    {
        Destroy(com.gameObject);
    }
}


