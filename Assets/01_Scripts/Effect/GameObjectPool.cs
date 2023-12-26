using UnityEngine;
using UnityEngine.Pool;
public class GameObjectPool : MonoBehaviour
{
    public int maxPoolSize = 5;
    public GameObject Prefab;
    public IObjectPool<GameObject> Pool { get; private set; }

    protected virtual void OnInitialize(GameObject go)
    {
        IPoolable poolable = go.GetComponent<IPoolable>();
        poolable.Init(Pool);
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
            OnDestroyPoolObject, true, maxPoolSize, maxPoolSize);

        for (int i = 0; i < maxPoolSize; i++)
        {
            OnInitialize(CreatePooledItem());
        }
    }
    
    GameObject CreatePooledItem() {
    {
        return Instantiate(Prefab, transform);
    }}

    void OnReturnedToPool(GameObject go)
    {
        go.SetActive(false);
    }

    void OnTakeFromPool(GameObject go)
    {
        go.SetActive(true);
    }

    void OnDestroyPoolObject(GameObject go)
    {
        Destroy(go);
    }
}


