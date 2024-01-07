using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using static System.Runtime.InteropServices.Marshal;

// [ExecuteInEditMode]
public class DashLineVisualization : MonoBehaviour, IPoolable
{
    [SerializeField] private ComputeShader dashLineCompute;
    [SerializeField] private Mesh instancedMesh;
    [SerializeField] private int instanceCount;
    
    private Bounds bounds;
    private Material instancedMaterial;

    private ComputeBuffer instancedBuffer;
    private ComputeBuffer randClipBuffer;
    private ComputeBuffer randColorBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    
    private static readonly int Instances = Shader.PropertyToID("_Instances");
    private static readonly int InstancedBuffer = Shader.PropertyToID("_InstanceBuffer");
    
    // dash setting
    [SerializeField] private float maxLifeTime = 0.5f;
    [SerializeField] private int centerBackOffset = 5;
    private float[] randArray;

    private float lifeTimer;
    private bool didCast;
    
    private IObjectPool<DashLineVisualization> pool;
    
    struct DashLine
    {
        Vector2 pos;
        Vector4 color;
    };

    
    public void Init<T>(IObjectPool<T> pool) where T : MonoBehaviour
    {
        this.pool = pool as IObjectPool<DashLineVisualization>;
        
        dashLineCompute = Instantiate(dashLineCompute);
        DontDestroyOnLoad(dashLineCompute);
        
        instancedMaterial = CoreUtils.CreateEngineMaterial("Hidden/DashLine");
        
        instancedBuffer = new ComputeBuffer(instanceCount, SizeOf(typeof(DashLine)));
        randClipBuffer = new ComputeBuffer(instanceCount, sizeof(float));
        randColorBuffer = new ComputeBuffer(instanceCount, sizeof(float));
        randArray = new float[instanceCount];
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
    }

    public void Cast(Vector2 posCenter, Vector2 dir)
    {
        ////////////// error test 
        // instance setting
        dashLineCompute.SetBuffer(0, Instances, instancedBuffer);
        
        // args setting
          args[0] = (uint)instancedMesh.GetIndexCount(0);
        args[1] = (uint)instanceCount;
        args[2] = (uint)instancedMesh.GetIndexStart(0);
        args[3] = (uint)instancedMesh.GetBaseVertex(0);
        argsBuffer.SetData(args);

        // final shader setting
        instancedMaterial.SetBuffer(InstancedBuffer, instancedBuffer);
        //////////////
        
        lifeTimer = maxLifeTime;
        didCast = true;

        var lineDir = Vector2.zero;
        if(dir.x != 0) lineDir.x = Mathf.Sign(dir.x);
        if(dir.y != 0) lineDir.y = Mathf.Sign(dir.y);
        
        Vector2 startPos = Vector2Int.RoundToInt(posCenter) - lineDir * centerBackOffset;
        
        int length = instanceCount;
        if (lineDir.magnitude > 1) 
            length = Mathf.RoundToInt(length * 0.7f); // 1 / root(2)
        
        // send
        for (int i = 0; i < randArray.Length; i++)
            randArray[i] = Random.value;
        randClipBuffer.SetData(randArray);
        dashLineCompute.SetBuffer(0, "_RandClipBuffer", randClipBuffer);
        
        for (int i = 0; i < instanceCount; i++)
            randArray[i] = Random.value;
        randColorBuffer.SetData(randArray);
        dashLineCompute.SetBuffer(0, "_RandColorBuffer", randColorBuffer);
        dashLineCompute.SetVector("_Direction", lineDir);
        dashLineCompute.SetFloat("_Length", length);
        dashLineCompute.SetFloat("_TotalTime", maxLifeTime);
        
        bounds = new Bounds(startPos, Vector3.one * 100f);
    }


    private void Update()
    {
        if (lifeTimer > 0f)
        {
            dashLineCompute.SetFloat("_LeftTime", lifeTimer);
            dashLineCompute.Dispatch(0, Mathf.CeilToInt(instanceCount / 256.0f), 1, 1);
            
            Graphics.DrawMeshInstancedIndirect(instancedMesh, 0, instancedMaterial, bounds, argsBuffer);
            lifeTimer -= Time.deltaTime;
        }
        else if (didCast)
        {
            didCast = false;
            pool.Release(this);
        }
    }

    private void OnDestroy()
    {
        argsBuffer?.Release();
        instancedBuffer?.Release();
        randClipBuffer?.Release();
        randColorBuffer?.Release();
        if(instancedMaterial) CoreUtils.Destroy(instancedMaterial);
        Destroy(dashLineCompute);
    }
} 
