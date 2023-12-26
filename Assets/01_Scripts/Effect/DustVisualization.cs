using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using static System.Runtime.InteropServices.Marshal;

public class DustVisualization : MonoBehaviour
{
    [FormerlySerializedAs("initializeShader")]
    [SerializeField] private ComputeShader dustCompute;
    [SerializeField] private Mesh instancedMesh;
    [SerializeField] private int instanceCount;
    
    private Bounds bounds;
    private Material instancedMaterial;

    private ComputeBuffer instancedBuffer;
    private ComputeBuffer randBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    
    private static readonly int Instances = Shader.PropertyToID("_Instances");
    private static readonly int InstancedBuffer = Shader.PropertyToID("_InstanceBuffer");
    
    private float aliveTimer;
    private float[] showRectRand;

    /// <summary>
    /// posWS bottom left side of rect
    /// </summary>
    public void Burst(Vector2 posCenter, Vector2 extent, Vector2 dir, float totalTime)
    {
        Debug.Log("S");
        aliveTimer = totalTime - Time.deltaTime;
        
        // decide random here!
        for (int i = 0; i < instanceCount; i++)
            showRectRand[i] = Random.value;
        
        // set buffer and dispatch(initialize)
        var rectInfo = new Vector4(-extent.x,-extent.y, extent.x * 2, extent.y * 2); 
            
        dustCompute.SetVector("_Rect", rectInfo);
        randBuffer.SetData(showRectRand);
        dustCompute.SetBuffer(0,"_ShowRectRand", randBuffer);
        dustCompute.SetFloat("_TotalTime", totalTime);
        dustCompute.SetFloat("_LeftTime", aliveTimer);
        dustCompute.SetVector("_Dir", dir * 8);
        
        dustCompute.Dispatch(0, Mathf.CeilToInt(instanceCount / 256.0f), 1, 1);
        bounds = new Bounds(posCenter, Vector2.one * 100f);
    }

    private void Start()
    {
        if (!instancedMaterial) instancedMaterial = CoreUtils.CreateEngineMaterial("Hidden/Dust");
        
        // randomArray
        showRectRand = new float[instanceCount];
        
        // instance setting
        instancedBuffer = new ComputeBuffer(instanceCount, SizeOf(typeof(Vector3)));
        randBuffer = new ComputeBuffer(instanceCount, sizeof(float));
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
            // set left time every frame
            dustCompute.SetFloat("_LeftTime", aliveTimer);
            dustCompute.Dispatch(0, Mathf.CeilToInt(instanceCount / 256.0f), 1, 1);

            aliveTimer -= Time.deltaTime;
            Graphics.DrawMeshInstancedIndirect(instancedMesh, 0, instancedMaterial, bounds, argsBuffer);
        }
    }

    private void OnDisable()
    {
        argsBuffer?.Release();
        instancedBuffer?.Release();
        randBuffer?.Release();
        if (instancedMaterial) CoreUtils.Destroy(instancedMaterial);
    }
} 
