
using System.Collections.Generic;
using UnityEngine;

public class MenuTerrain : MonoBehaviour
{
    [SerializeField] private List<TerrainGenerator> generators;

    void GrabGenerators()
    {
        generators.Clear();
        foreach (Transform child in transform)
        {
            generators.Add(child.GetComponent<TerrainGenerator>());
        }
    }
    
    public void Initiate()
    {
        GrabGenerators();
        
        float globalMinHeight = float.PositiveInfinity;
        float globalMaxHeight = float.NegativeInfinity;
        
        foreach (var generator in generators)
        {
            generator.OverrideMinMaxHeight(globalMinHeight, globalMaxHeight);
            
            generator.Initiate();

            globalMinHeight = Mathf.Min(globalMinHeight, generator.MinNoiseHeight);
            globalMaxHeight = Mathf.Max(globalMaxHeight, generator.MaxNoiseHeight);
        }
    }

    public void SaveMenuTerrains()
    {
        int i = 0;
        foreach (var generator in generators)
        {
            generator.SaveMesh(i++);
        }
    }
}


