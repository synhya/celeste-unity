using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using static System.Runtime.InteropServices.Marshal;

// [ExecuteInEditMode]
public class DashLineVisualization : MonoBehaviour
{
    [SerializeField] private ComputeShader initializeShader;
    [SerializeField] private Mesh instancedMesh;
    [SerializeField] private int instanceCount;
    
    private Bounds bounds;
    private Material instancedMaterial;

    private ComputeBuffer instancedBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    
    private static readonly int Instances = Shader.PropertyToID("_Instances");
    private static readonly int InstancedBuffer = Shader.PropertyToID("_InstanceBuffer");

    private struct InstanceData
    {
        public Vector3 position;
    }

    private IObjectPool<GameObject> pool;
    
    public void Init(IObjectPool<GameObject> pool)
    {
        this.pool = pool;
        this.pool.Release(gameObject);
        
    }

    private void OnEnable()
    {
        if (!instancedMaterial) instancedMaterial = CoreUtils.CreateEngineMaterial("Hidden/DashLineVisualization");
        
        bounds = new Bounds(transform.position, Vector3.one * 100f);

        // instance setting
        instancedBuffer = new ComputeBuffer(instanceCount, SizeOf(typeof(InstanceData)));
        initializeShader.SetBuffer(0, Instances, instancedBuffer);
        initializeShader.Dispatch(0, Mathf.CeilToInt(instanceCount / 256.0f), 1, 1);
        
        
        // args setting
        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        args[0] = (uint)instancedMesh.GetIndexCount(0);
        args[1] = (uint)instanceCount;
        args[2] = (uint)instancedMesh.GetIndexStart(0);
        args[3] = (uint)instancedMesh.GetBaseVertex(0);
        argsBuffer.SetData(args);

        // final shader setting
        instancedMaterial.SetBuffer(InstancedBuffer, instancedBuffer);
    }


    private void Update()
    {
        if (!Application.isPlaying) 
        {
            OnDisable();
            OnEnable();
        }
        
        if(instancedMaterial)
            Graphics.DrawMeshInstancedIndirect(instancedMesh, 0, instancedMaterial, bounds, argsBuffer);
    }

    private void OnDisable()
    {
        argsBuffer?.Release();
        instancedBuffer?.Release();
        if(instancedMaterial) CoreUtils.Destroy(instancedMaterial);
    }
} 
