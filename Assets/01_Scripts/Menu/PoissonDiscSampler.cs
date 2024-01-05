
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiscSampler
{
    public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
    {
        Random.InitState(10);
        float cellSize = radius / Mathf.Sqrt(2);

        // grid maps to points array idx. 
        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();
        
        spawnPoints.Add(sampleRegionSize/ 2);

        while (spawnPoints.Count != 0)
        {
            // pick candidates around spawn point.
            int spawnIdx = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIdx];

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCenter + dir * Random.Range(radius, 2 * radius);

                if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    spawnPoints.Add(candidate);

                    break;
                }
                
                if(i == numSamplesBeforeRejection - 1) 
                    spawnPoints.RemoveAt(spawnIdx);
            }
        }

        return points;
    }

    static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndX = Mathf.Min(cellX + 2 , grid.GetLength(0) - 1);
            int searchEndY = Mathf.Min(cellY + 2 , grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIdx = grid[x, y] - 1;
                    if (pointIdx != -1)
                    {
                        float sqrDst = (candidate - points[pointIdx]).sqrMagnitude;
                        if (sqrDst < radius * radius)
                        {
                            return false;
                        }
                    }
                }
            }
            
            return true;
        }

        return false;
    }
}


