using System.Collections;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);

        texture.filterMode = FilterMode.Point; //KANSKE VILL ÄNDRAS SENARE!!!

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D DrawMap(int size, MapSettings mapSettings, Vector2 center, int noiseViewSize = 1)
    {
        float[,] heightNoise = Noise.GenerateNoiseMap(size * noiseViewSize, size * noiseViewSize, mapSettings.DetailLevel, mapSettings.HeightRegion.NoiseData.NoiseSettingsDataMerge, center);
        float[][,] noises = new float[mapSettings.MapRegions.Length][,];

        for (int i = 0; i < mapSettings.MapRegions.Length; i++)
        {
            noises[i] = Noise.GenerateNoiseMap(size * noiseViewSize, size * noiseViewSize, mapSettings.DetailLevel, mapSettings.MapRegions[i].NoiseData.NoiseSettingsDataMerge, center);
        }

        int width = noises[0].GetLength(0);
        int height = noises[0].GetLength(0);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = heightNoise[x, y];

                for (int h = 0; h < mapSettings.HeightRegion.Regions.Length; h++)
                {
                    if (currentHeight <= mapSettings.HeightRegion.Regions[h].height)
                    {
                        Color finalColor = mapSettings.HeightRegion.Regions[h].color;

                        for (int i = 0; i < noises.Length; i++)
                        {
                            if (noises[i][x, y] >= mapSettings.MapRegions[i].NoiseStartPoint && currentHeight > mapSettings.MapRegions[i].MinHeightStart)
                            {
                                finalColor = finalColor.grayscale * mapSettings.MapRegions[i].Color;
                                break;
                            }
                        }

                        colorMap[x * width + y] = finalColor;

                        break;
                    }
                }
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TextureFromNoise(float[,] noise)
    {
        int width = noise.GetLength(0);
        int height = noise.GetLength(1);

        Color[] colorMap = new Color[width * height];

        float maxValue;
        float minValue;

        GetMinMax(noise, out minValue, out maxValue);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(0, maxValue, noise[x, y]));

        return TextureFromColorMap(colorMap, width, height);
    }

    private static void GetMinMax(float[,] noise, out float minValue, out float maxValue)
    {
        minValue = float.MaxValue;
        maxValue = float.MinValue;

        int width = noise.GetLength(0);
        int height = noise.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                if (noise[x, y] < minValue)
                    minValue = noise[x, y];
                if (noise[x, y] > maxValue)
                    maxValue = noise[x, y];
            }
        }
    }
}
