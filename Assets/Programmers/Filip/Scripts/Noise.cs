using System.Collections;
using UnityEngine;

//Create a 2d noise map 
public static class Noise
{
    //Constant to try and guess height in global normalize mode
    static readonly float GLOBAL_MODE_ESTIMATE_MULTIPLIER = 2.25f;

    public enum NormalizeMode
    {
        LOCAL,
        GLOBAL
    }

    public static float[,] GenerateNoiseMap(NormalizeMode normalizeMode, int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {

        float[,] noiseMap = new float[mapWidth, mapHeight];

        //Used as offset based on seed to get new noise
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        //Sets the offsets based on seed
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude += persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
            Debug.LogWarning("Scale is smaller or equal to zero! Don't! Clamped to 0.0001 for now!");
        }

        //keep track of max and min to be able to normalize 0-1
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        //Allow scale to scale from middle
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //amplitude and frequency are changed per octave
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                //sets current min and max value in float[,]
                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                else if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        //Normalize values in noiseMap
        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                if (normalizeMode == NormalizeMode.LOCAL)
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / GLOBAL_MODE_ESTIMATE_MULTIPLIER);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }

        return noiseMap;
    }
}
