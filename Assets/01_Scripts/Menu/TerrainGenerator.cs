
using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{

    [SerializeField] private Material terrainMat;
        
    [Header("Point Distribution")]
    [SerializeField] [Range(1, 1000)] private int sizeX = 5;
    [SerializeField] [Range(1, 1000)] private int sizeY = 5;
    [SerializeField] [Range(0.001f, 100)] private float poissonRadius = 1;
    [SerializeField][Range(5, 30)] private int beforeReject = 30;

    [Header("Color")]
    public Gradient heightGradient;

    [Header("Perlin Noise")]
    [SerializeField][Range(1, 3000f)] private float heightScale = 300;
    [SerializeField] [Range(1, 500)] private float noiseScale = 300;
    [SerializeField] [Range(0.001f, 1f)] private float dampening = 0.1f;
    
    [Header("Layered Noise")]
    [SerializeField] [Range(1, 15)] private int octaves = 3;
    [SerializeField] [Range(0, 1)] private float persistence = 0.1f;
    [SerializeField] [Range(1, 10)] private float lacunarity = 2f;

    [Header("Additional Texture")]
    [SerializeField] private Texture2D heightTexture;
    [SerializeField] private Texture2D lowerTexture;
    [FormerlySerializedAs("texAdjustValue")]
    [FormerlySerializedAs("texValue")]
    [SerializeField] [Range(0, 1)] private float heightTexRatio = 1;
    [SerializeField] [Range(0, 1)] private float lowerTexRatio = 1;

    
    public int seed;

    private Polygon polygon;
    private TriangleNet.Mesh mesh;
    private UnityEngine.Mesh terrainMesh;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private List<Vector2> poissonPoints;
    private List<float> heights;
    private float minNoiseHeight;
    private float maxNoiseHeight;

    private float maxPossibleHeight;
    private float minPossibleHeight;

    private Vector2 posOffset;

    // [ContextMenu("Surround Create")]
    // void NineSurround()
    // {
    //     for (int i = 0; i < 3; i++)
    //     {
    //         for (int j = 0; j < 3; j++)
    //         {
    //             posOffset
    //             Initiate(false);
    //         }
    //     }
    // }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
            Initiate();
    }

    public void Initiate()
    {
        posOffset = new Vector2(transform.position.x, transform.position.z);
        
        // get component
        meshFilter = transform.GetComponent<MeshFilter>();
        meshRenderer = transform.GetComponent<MeshRenderer>();
        meshCollider = transform.GetComponent<MeshCollider>();
        
        // mesh init part
        polygon = new Polygon();

        poissonPoints = PoissonDiscSampler.GeneratePoints(poissonRadius, new Vector2(sizeX, sizeY), beforeReject);
        for(int i = 0; i < poissonPoints.Count; i++) 
            polygon.Add(new Vertex(poissonPoints[i].x, poissonPoints[i].y));

        ConstraintOptions constaints = new ConstraintOptions
        {
            ConformingDelaunay = true
        };
        
        mesh = polygon.Triangulate(constaints) as TriangleNet.Mesh;

        ShapeTerrains();
        GenerateMesh();
    }

    void ShapeTerrains()
    {
        heights = new List<float>();
        
        minNoiseHeight = float.PositiveInfinity;
        maxNoiseHeight = float.NegativeInfinity;

        bool useAdditionalTex = heightTexture != null;

        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            float amplitude = 1f;
            float frequency = 1f;
            float noiseHeight = 0f;

            for (int o = 0; o < octaves; o++)
            {
                float xValue = ((float)mesh.vertices[i].x + posOffset.x)  / noiseScale * frequency;
                float yValue = ((float)mesh.vertices[i].y + posOffset.y) / noiseScale * frequency;
                float perlinValue = Mathf.PerlinNoise(xValue + seed, yValue  + seed) * 2 - 1; // -1 ~ 1
                perlinValue *= dampening;
                
                noiseHeight += perlinValue * amplitude;
                amplitude *= persistence; // 기존값에 추가되는 정도 (감쇠)
                frequency *= lacunarity; // 위로 갈수록 주기가 짧아지는게 가파라서 자연스러울듯.

                maxPossibleHeight += amplitude;
                minPossibleHeight -= amplitude;
            }

            if (useAdditionalTex)
            {
                int coordX = Mathf.RoundToInt((float)(mesh.vertices[i].x * (heightTexture.width / sizeX)));
                int coordY = Mathf.RoundToInt((float)(mesh.vertices[i].y * (heightTexture.height / sizeY)));
                noiseHeight += heightTexture.GetPixel(coordX, coordY).r * heightTexRatio;
                noiseHeight -= lowerTexture.GetPixel(coordX, coordY).r * lowerTexRatio;

                maxPossibleHeight += heightTexRatio;
                minPossibleHeight -= lowerTexRatio;
            }
            

            if (noiseHeight > maxNoiseHeight)
                maxNoiseHeight = noiseHeight;
            else if (noiseHeight < minNoiseHeight)
                minNoiseHeight = noiseHeight;

            noiseHeight = (noiseHeight < 0f) ? noiseHeight * heightScale / 10f : noiseHeight * heightScale;
            
            heights.Add(noiseHeight);
        }
    }

    void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        IEnumerator<Triangle> triangleEnum = mesh.Triangles.GetEnumerator();

        for (int i = 0; i < mesh.Triangles.Count; i++)
        {
            if(!triangleEnum.MoveNext()) 
                break;

            Triangle triangle = triangleEnum.Current;
            
            // lefthand
            Vector3 v0 = new Vector3((float)triangle.vertices[2].x, heights[triangle.vertices[2].id], (float)triangle.vertices[2].y);
            Vector3 v1 = new Vector3((float)triangle.vertices[1].x, heights[triangle.vertices[1].id], (float)triangle.vertices[1].y);
            Vector3 v2 = new Vector3((float)triangle.vertices[0].x, heights[triangle.vertices[0].id], (float)triangle.vertices[0].y);
            
            triangles.Add(vertices.Count);
            triangles.Add(vertices.Count + 1);
            triangles.Add(vertices.Count + 2);
            
            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
            
            // righthand
            var normal = Vector3.Cross(v1 - v0, v2 - v0);

            var color = EvaluateColor(triangle);

            for (int x = 0; x < 3; x++)
            {
                normals.Add(normal);
                uvs.Add(Vector3.zero);
                colors.Add(color);
            }
        }
        
        // pass to unity terrain mesh
        terrainMesh = new Mesh()
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            normals = normals.ToArray(),
            uv = uvs.ToArray(),
            colors = colors.ToArray()
        };

        meshFilter.mesh = terrainMesh;
        meshRenderer.material = terrainMat;
        meshCollider.sharedMesh = terrainMesh;
        
        triangleEnum.Dispose();
    }
    private Color EvaluateColor(Triangle triangle)
    {
        var currentHeight = heights[triangle.vertices[0].id] + heights[triangle.vertices[1].id] + heights[triangle.vertices[2].id];
        currentHeight /= 3f;

        currentHeight = (currentHeight < 0f) ? currentHeight / heightScale * 10f : currentHeight / heightScale;

        float gradientVal = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, currentHeight);
        
        return heightGradient.Evaluate(gradientVal);
    }
    public void SaveMesh()
    {
        if (transform.GetComponent<MeshFilter>() != null)
        {
            var path = "Assets/GeneratedMesh" + seed + ".asset";
            AssetDatabase.CreateAsset(transform.GetComponent<MeshFilter>().sharedMesh, path);
        }
    }
}


