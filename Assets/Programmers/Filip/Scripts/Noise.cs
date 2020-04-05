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

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettingsDataMerge[] settingsMerge, Vector2 sampleCenter)
    {
        float[,] noise = GenerateNoiseMap(mapWidth, mapHeight, settingsMerge[0].NoiseSettings, sampleCenter);

        for (int i = 1; i < settingsMerge.Length; i++)
        {
            noise = MergeNoise(mapWidth, mapHeight, noise, settingsMerge[i].NoiseSettings, settingsMerge[i - 1].MoiseMergeType, sampleCenter);
        }

        return noise;
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

                if (settings.UseDarken)
                {
                    noiseHeight -= settings.Darken;

                    if (noiseHeight <= 0)
                        noiseHeight = 0;
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

    public static float[,] MergeNoise(int width, int height, float[,] noise_1, float[,] noise_2, NoiseMergeType noiseMergeType, Vector2 center)
    {
        float[,] finalNoise = new float[width, height];
        float maxHeight = 1f;
        float minHeight = 0f;

        if (noiseMergeType == NoiseMergeType.ONLY_FIRST)
            finalNoise = noise_1;
        else if (noiseMergeType == NoiseMergeType.ONLY_SECOND)
            finalNoise = noise_2;
        else
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (noiseMergeType == NoiseMergeType.ADD)
                    {
                        finalNoise[i, j] = noise_1[i, j] + noise_2[i, j];
                        if (i == 0 && j == 0)
                        {
                            maxHeight = 2f;
                            minHeight = 0f;
                        }
                    }
                    else if (noiseMergeType == NoiseMergeType.MULTIPLY)
                        finalNoise[i, j] = noise_1[i, j] * noise_2[i, j];
                    else if (noiseMergeType == NoiseMergeType.SUBTRACT)
                    {
                        finalNoise[i, j] = noise_1[i, j] - noise_2[i, j];
                        if (i == 0 && j == 0)
                        {
                            maxHeight = 1f;
                            minHeight = -1f;
                        }
                    }
                    else if (noiseMergeType == NoiseMergeType.DIVIDE)
                    {
                        if (noise_2[i, j] != 0)
                            finalNoise[i, j] = noise_1[i, j] / noise_2[i, j];
                        else
                            finalNoise[i, j] = noise_1[i, j] / 0.0001f;
                        if (i == 0 && j == 0)
                        {
                            maxHeight = 1 / 0.0001f;
                            minHeight = 0f;
                        }
                    }
                }
            }
        }

        //Normalize 
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                finalNoise[x, y] = Mathf.InverseLerp(minHeight, maxHeight, finalNoise[x, y]);
            }

        return finalNoise;
    }

    public static float[,] MergeNoise(int mapWidth, int mapHeight, NoiseSettingsDataMerge[] settingsMerge_1, NoiseSettingsDataMerge[] settingsMerge_2, NoiseMergeType noiseMergeType, Vector2 sampleCenter)
    {
        float[,] noise_1 = GenerateNoiseMap(mapWidth, mapHeight, settingsMerge_1, sampleCenter);
        float[,] noise_2 = GenerateNoiseMap(mapWidth, mapHeight, settingsMerge_2, sampleCenter);

        return MergeNoise(mapWidth, mapHeight, noise_1, noise_2, noiseMergeType, sampleCenter);
    }

    public static float[,] MergeNoise(int mapWidth, int mapHeight, NoiseSettingsDataMerge[] settingsMerge_1, float[,] noise_2, NoiseMergeType noiseMergeType, Vector2 sampleCenter)
    {
        float[,] noise_1 = GenerateNoiseMap(mapWidth, mapHeight, settingsMerge_1, sampleCenter);

        return MergeNoise(mapWidth, mapHeight, noise_1, noise_2, noiseMergeType, sampleCenter);
    }

    public static float[,] MergeNoise(int width, int height, NoiseSettings noiseSettings_1, NoiseSettings noiseSettings_2, NoiseMergeType noiseMergeType, Vector2 center)
    {
        float[,] noise_1 = GenerateNoiseMap(width, height, noiseSettings_1, center);
        float[,] noise_2 = GenerateNoiseMap(width, height, noiseSettings_2, center);

        return MergeNoise(width, height, noise_1, noise_2, noiseMergeType, center);
    }

    public static float[,] MergeNoise(int width, int height, float[,] noise_1, NoiseSettings noiseSettings_2, NoiseMergeType noiseMergeType, Vector2 center)
    {
        float[,] noise_2 = GenerateNoiseMap(width, height, noiseSettings_2, center);

        return MergeNoise(width, height, noise_1, noise_2, noiseMergeType, center);
    }

    public static float[,] MergeNoise(int width, int height, NoiseSettings noiseSettings_1, float[,] noise_2, NoiseMergeType noiseMergeType, Vector2 center)
    {
        float[,] noise_1 = GenerateNoiseMap(width, height, noiseSettings_1, center);

        return MergeNoise(width, height, noise_1, noise_2, noiseMergeType, center);
    }
}

[System.Serializable]
public class NoiseSettings
{
    [SerializeField, Range(0, 100000), Tooltip("Gives a different starting point for noise")] int _seed;
    [SerializeField, Tooltip("Offset from noise seed if desired")] Vector2 _offset;
    [SerializeField, Range(0.001f, 1000), Tooltip("Scale of Noise")] float _scale = 10;
    [SerializeField, Range(1, 10), Tooltip("Amount of noiselayers stacked on eachother (3 is good)")] int _octaves = 4;
    [SerializeField, Range(0, 1), Tooltip("Amount of increase in frequency of each octave")] float _persistance = 0.6f;
    [SerializeField, Range(1, 25), Tooltip("How much effect each octave should have")] float _lacunarity = 1.5f;

    [SerializeField, Range(-1, 1)] float _darken = 0.0f;
    [SerializeField] bool _useDarken = false;

    [SerializeField, Tooltip("Keep on GLOBAL! LOCAL is only used for testing purposes!")] Noise.NormalizeMode _normalizeMode = Noise.NormalizeMode.GLOBAL;

    public int Seed                          { get { return _seed; }          private set { _seed = value;  } }
    public Vector2 Offset                    { get { return _offset; }        private set { _offset = value;  } }
    public float Scale                       { get { return _scale; }         private set { _scale = value; } }
    public int Octaves                       { get { return _octaves; }       private set { _octaves = value;  } }
    public float Persistance                 { get { return _persistance; }   private set { _persistance = value;  } }
    public float Lacunarity                  { get { return _lacunarity; }    private set { _lacunarity = value;  } }
    public float Darken                      { get { return _darken; }        private set { _darken = value; } }
    public bool UseDarken                    { get { return _useDarken; }     private set { _useDarken = value; } }
    public Noise.NormalizeMode NormalizeMode { get { return _normalizeMode; } private set { _normalizeMode = value;  } }
}

public enum NoiseMergeType
{
    ONLY_FIRST,
    ONLY_SECOND,
    MULTIPLY,
    ADD,
    SUBTRACT,
    DIVIDE
}
