using UnityEngine;
using UnityEngine.Rendering;
using static System.Runtime.InteropServices.Marshal;

public class DustVisualization : MonoBehaviour
{
    [SerializeField] private ComputeShader initializeShader;
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

    [Header("Dust settings")]
    public RectInt rect = new RectInt(-4, -4, 8, 8);

    private float aliveTimer;
    private float[] showRectRand;

    public void Burst(Vector2 posWS, Vector2 dir, float totalTime)
    {
        aliveTimer = totalTime;
        
        // decide random here!
        for (int i = 0; i < instanceCount; i++)
            showRectRand[i] = Random.value;
        
        // set buffer and dispatch(initialize)
        var p = rect.position + posWS;
        
        var rectInfo = new Vector4(p.x, p.y, rect.size.x, rect.size.y);
        initializeShader.SetVector("_Rect", rectInfo);
        randBuffer.SetData(showRectRand);
        initializeShader.SetBuffer(0,"_ShowRectRand", randBuffer);
        initializeShader.SetFloat("_TotalTime", totalTime);
        initializeShader.SetFloat("_LeftTime", aliveTimer);
        initializeShader.SetVector("_Dir", dir * 8);
        
        initializeShader.Dispatch(0, Mathf.CeilToInt(instanceCount / 256.0f), 1, 1);
        bounds = new Bounds(transform.position, Vector2.one * 100f);
    }

    private void Start()
    {
        if (!instancedMaterial) instancedMaterial = CoreUtils.CreateEngineMaterial("Hidden/Dust");
        
        // randomArray
        showRectRand = new float[instanceCount];
        
        // instance setting
        instancedBuffer = new ComputeBuffer(instanceCount, SizeOf(typeof(Vector3)));
        randBuffer = new ComputeBuffer(instanceCount, sizeof(float));
        initializeShader.SetBuffer(0, Instances, instancedBuffer);

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
            initializeShader.SetFloat("_LeftTime", aliveTimer);
            initializeShader.Dispatch(0, Mathf.CeilToInt(instanceCount / 256.0f), 1, 1);

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
