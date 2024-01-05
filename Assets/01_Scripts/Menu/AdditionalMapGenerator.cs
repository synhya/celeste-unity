
using UnityEngine;

public static class AdditionalMapGenerator
{
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                
            }
        }

        return map;
    }
    public static float[,] GenerateMapFromTexture(Texture2D tex, int sizeX, int sizeY)
    {
        float[,] map = new float[sizeX, sizeY];

        if (tex)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    int xCoord = Mathf.RoundToInt(x * (float)tex.width / sizeX); //텍스처 값과 크기 값에 맞춰 좌표 저장
                    int yCoord = Mathf.RoundToInt(y * (float)tex.height / sizeY);
                    map[x, y] = tex.GetPixel(xCoord, yCoord).grayscale; //텍스처에서 색상을 가져와 그레이 스케일로 배열에 저장
                }
            }
        }
        
        return map;
    }
}

