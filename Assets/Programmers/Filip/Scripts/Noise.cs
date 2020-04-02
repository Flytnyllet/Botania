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

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter)
    {

        float[,] noiseMap = new float[mapWidth, mapHeight];

        //Used as offset based on seed to get new noise
        System.Random prng = new System.Random(settings.Seed);
        Vector2[] octaveOffsets = new Vector2[settings.Octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        //Sets the offsets based on seed
        for (int i = 0; i < settings.Octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.Offset.x + sampleCenter.x;
            float offsetY = prng.Next(-100000, 100000) - settings.Offset.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude += settings.Persistance;
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

                for (int i = 0; i < settings.Octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.Scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.Scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.Persistance;
                    frequency *= settings.Lacunarity;
                }
                //sets current min and max value in float[,]
                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;

                if (settings.NormalizeMode == NormalizeMode.GLOBAL)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / GLOBAL_MODE_ESTIMATE_MULTIPLIER);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        //Normalize values in noiseMap
        if (settings.NormalizeMode == NormalizeMode.LOCAL)
        {
            for (int y = 0; y < mapHeight; y++)
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
        }
            
        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    [SerializeField, Range(0, 100000), Tooltip("Gives a different starting point for noise")] int _seed;
    [SerializeField, Tooltip("Offset from noise seed if desired")] Vector2 _offset;
    [SerializeField, Range(0.001f, 100), Tooltip("Scale of Noise")] float _scale = 10;
    [SerializeField, Range(1, 10), Tooltip("Amount of noiselayers stacked on eachother (3 is good)")] int _octaves = 4;
    [SerializeField, Range(0, 1), Tooltip("Amount of increase in frequency of each octave")] float _persistance = 0.6f;
    [SerializeField, Range(1, 25), Tooltip("How much effect each octave should have")] float _lacunarity = 1.5f;

    [SerializeField, Tooltip("Keep on GLOBAL! LOCAL is only used for testing purposes!")] Noise.NormalizeMode _normalizeMode = Noise.NormalizeMode.GLOBAL;

    public int Seed { get { return _seed; } private set { _seed = value;  } }
    public Vector2 Offset { get { return _offset; } private set { _offset = value;  } }
    public float Scale { get { return _scale; } private set { _scale = value; } }
    public int Octaves { get { return _octaves; } private set { _octaves = value;  } }
    public float Persistance { get { return _persistance; } private set { _persistance = value;  } }
    public float Lacunarity { get { return _lacunarity; } private set { _lacunarity = value;  } }
    public Noise.NormalizeMode NormalizeMode { get { return _normalizeMode; } private set { _normalizeMode = value;  } }
}
