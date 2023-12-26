using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using static System.Runtime.InteropServices.Marshal;
using Random = UnityEngine.Random;

public class DustVisualization : MonoBehaviour, IPoolable
{
    [SerializeField] private ComputeShader dustCompute;
    [SerializeField] private Mesh instancedMesh;
    private Material instancedMaterial;
    [SerializeField] private int instanceCount = 50;
    [SerializeField][Range(0, 1)] private float boxierValueX;
    [SerializeField][Range(0, 1)] private float boxierValueY;

    private Bounds bounds;

    private ComputeBuffer instancedBuffer;
    private ComputeBuffer randBuffer;
    private float[] showRectRand;
    
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    
    private static readonly int Instances = Shader.PropertyToID("_Instances");
    private static readonly int InstancedBuffer = Shader.PropertyToID("_InstanceBuffer");
    
    private float aliveTimer;
    private float totalAliveTime;
    private Vector2 speed;
    private Vector4 rectInfo;
    // for pooling
    private IObjectPool<GameObject> pool;
    private bool doBurst = false;
    
    /// <summary>
    /// posWS bottom left side of rect
    /// </summary>
    public void Burst(Vector2 posCenter, Vector2 extent, Vector2 dir, float totalTime)
    {
        totalAliveTime = totalTime;
        aliveTimer = totalAliveTime;
        
        doBurst = true;
        speed = dir * 8;
        
        rectInfo = new Vector4(-extent.x,-extent.y, extent.x * 2, extent.y * 2); 
        
        // decide random here!
        for (int i = 0; i < instanceCount; i++)
            showRectRand[i] = Random.value;
        randBuffer.SetData(showRectRand);
        
        dustCompute.SetVector("_Rect", rectInfo);
        dustCompute.SetBuffer(0,"_ShowRectRand", randBuffer);
        dustCompute.SetFloat("_TotalTime", totalAliveTime);
        dustCompute.SetVector("_Dir", speed);
        dustCompute.SetVector("_BoxierValue", new Vector2(boxierValueX, boxierValueY));
        
        bounds = new Bounds(posCenter, Vector2.one * 100f);
    }

    public void Init(IObjectPool<GameObject> pool)
    {
        this.pool = pool;
        this.pool.Release(gameObject);
        
        dustCompute = Instantiate(dustCompute);
        instancedMaterial = CoreUtils.CreateEngineMaterial("Hidden/Dust");
        
        instancedBuffer = new ComputeBuffer(instanceCount, SizeOf(typeof(Vector3)));
        randBuffer = new ComputeBuffer(instanceCount, sizeof(float));
        showRectRand = new float[instanceCount];
        
        dustCompute.SetBuffer(0, Instances, instancedBuffer);
        
        // args setting
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        args[0] = (uint)instancedMesh.GetIndexCount(0);
        args[1] = (uint)instanceCount;
        args[2] = (uint)instancedMesh.GetIndexStart(0);
        args[3] = (uint)instancedMesh.GetBaseVertex(0);
        argsBuffer.SetData(args);
        
        instancedMaterial.SetBuffer(InstancedBuffer, instancedBuffer);
    }

    private void Update()
    {
        if (aliveTimer > 0f)
        {
            aliveTimer -= Time.deltaTime;
            dustCompute.SetFloat("_LeftTime", aliveTimer);
            dustCompute.Dispatch(0, Mathf.CeilToInt(instanceCount / 256.0f), 1, 1);
            
            Graphics.DrawMeshInstancedIndirect(instancedMesh, 0, instancedMaterial, bounds, argsBuffer);
        } 
        
        else if (doBurst)
        {
            pool?.Release(gameObject);
        }
    }

    private void OnDestroy()
    {
        argsBuffer?.Release();
        instancedBuffer?.Release();
        randBuffer?.Release();
        CoreUtils.Destroy(instancedMaterial);
    }
} 
