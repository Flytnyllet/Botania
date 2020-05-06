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

    //Combines different noises of one noise object into one final noise
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int detailLevel, NoiseSettingsDataMerge[] settingsMerge, Vector2 sampleCenter)
    {
        float[,] noise = GenerateNoiseMap(mapWidth, mapHeight, detailLevel, settingsMerge[0].NoiseSettings, sampleCenter);

        for (int i = 1; i < settingsMerge.Length; i++)
        {
            noise = MergeNoise(mapWidth, mapHeight, detailLevel, noise, settingsMerge[i].NoiseSettings, settingsMerge[i - 1].MoiseMergeType, sampleCenter);
        }

        return noise;
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int detailLevel, NoiseSettings settings, Vector2 sampleCenter)
    {
        if (detailLevel <= 0)
        {
            Debug.LogError("Detail level of noise may not be lower than 1!!");
            detailLevel = 1;
        }

        float[,] noiseMap = new float[mapWidth / detailLevel, mapHeight / detailLevel];

        //This justs set everything to white and returns it
        if (settings.AllWhite)
        {
            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                for (int y = 0; y < noiseMap.GetLength(1); y++)
                {
                    noiseMap[x, y] = 1.0f;
                }
            }

            return noiseMap;
        }

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

        maxPossibleHeight = (maxPossibleHeight + settings.AddValue) * settings.Strength;

        //keep track of max and min to be able to normalize 0-1
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        //Allow scale to scale from middle
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y += detailLevel)
        {
            for (int x = 0; x < mapWidth; x += detailLevel)
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

                    if (settings.NormalizeMode == NormalizeMode.LOCAL && noiseHeight < 0)
                        noiseHeight = 0;
                }

                //sets current min and max value in float[,]
                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;

                if (x / detailLevel < mapWidth / detailLevel && y / detailLevel < mapHeight / detailLevel)
                {
                    noiseMap[x / detailLevel, y / detailLevel] = noiseHeight + settings.AddValue;
                    noiseMap[x / detailLevel, y / detailLevel] *= settings.Strength;
                }
            }
        }

        //Normalize values in noiseMap

        for (int y = 0; y < noiseMap.GetLength(0); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(1); x++)
            {
                if (settings.NormalizeMode == NormalizeMode.LOCAL)
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / GLOBAL_MODE_ESTIMATE_MULTIPLIER);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        noiseMap = SetupNoise(noiseMap, settings.MaskValue, settings.Mask, settings.Invert);

        return noiseMap;
    }

    //Base for all merge noise functions, combine two different noise according to mode
    public static float[,] MergeNoise(int width, int height, float[,] noise_1, float[,] noise_2, NoiseMergeType noiseMergeType, Vector2 center, bool mask)
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

                        if (mask)
                            finalNoise[i, j] = Mathf.Clamp(finalNoise[i, j], 0, 1);
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


        ////Normalize 


        //for (int y = 0; y < height; y++)
        //    for (int x = 0; x < width; x++)
        //    {
        //        finalNoise[x, y] = Mathf.InverseLerp(minHeight, maxHeight, finalNoise[x, y]);
        //    }

        return finalNoise;
    }

    public static float[,] SetupNoise(float[,] noise, float maskValue, bool mask, bool invert)
    {
        if (mask || invert)
        {
            for (int x = 0; x < noise.GetLength(0); x++)
            {
                for (int y = 0; y < noise.GetLength(1); y++)
                {
                    noise[x, y] = noise[x, y] >= maskValue ? invert ? 0 : 1 : invert ? 1 : 0;
                }
            }
        }
        
        return noise;
    }

    //Converts different input parameter data and sends it to base merge function ^
    public static float[,] MergeNoise(int mapWidth, int mapHeight, int detailLevel, NoiseSettingsDataMerge[] settingsMerge_1, NoiseSettingsDataMerge[] settingsMerge_2, NoiseMergeType noiseMergeType, Vector2 sampleCenter)
    {
        float[,] noise_1 = GenerateNoiseMap(mapWidth, mapHeight, detailLevel, settingsMerge_1, sampleCenter);
        float[,] noise_2 = GenerateNoiseMap(mapWidth, mapHeight, detailLevel, settingsMerge_2, sampleCenter);

        return MergeNoise(mapWidth / detailLevel, mapHeight / detailLevel, noise_1, noise_2, noiseMergeType, sampleCenter, false);
    }
    public static float[,] MergeNoise(int mapWidth, int mapHeight, int detailLevel, NoiseSettingsDataMerge[] settingsMerge_1, float[,] noise_2, NoiseMergeType noiseMergeType, Vector2 sampleCenter)
    {
        float[,] noise_1 = GenerateNoiseMap(mapWidth, mapHeight, detailLevel, settingsMerge_1, sampleCenter);

        return MergeNoise(mapWidth / detailLevel, mapHeight / detailLevel, noise_1, noise_2, noiseMergeType, sampleCenter, false);
    }
    public static float[,] MergeNoise(int width, int height, int detailLevel, NoiseSettings noiseSettings_1, NoiseSettings noiseSettings_2, NoiseMergeType noiseMergeType, Vector2 center)
    {
        float[,] noise_1 = GenerateNoiseMap(width, height, detailLevel, noiseSettings_1, center);
        float[,] noise_2 = GenerateNoiseMap(width, height, detailLevel, noiseSettings_2, center);

        return MergeNoise(width / detailLevel, height / detailLevel, noise_1, noise_2, noiseMergeType, center, noiseSettings_1.Mask && noiseSettings_2.Mask);
    }
    public static float[,] MergeNoise(int width, int height, int detailLevel, float[,] noise_1, NoiseSettings noiseSettings_2, NoiseMergeType noiseMergeType, Vector2 center)
    {
        float[,] noise_2 = GenerateNoiseMap(width, height, detailLevel, noiseSettings_2, center);

        return MergeNoise(width / detailLevel, height / detailLevel, noise_1, noise_2, noiseMergeType, center, noiseSettings_2.Mask);
    }
    public static float[,] MergeNoise(int width, int height, int detailLevel, NoiseSettings noiseSettings_1, float[,] noise_2, NoiseMergeType noiseMergeType, Vector2 center)
    {
        float[,] noise_1 = GenerateNoiseMap(width, height, detailLevel, noiseSettings_1, center);

        return MergeNoise(width / detailLevel, height / detailLevel, noise_1, noise_2, noiseMergeType, center, noiseSettings_1.Mask);
    }
}

[System.Serializable]
public class NoiseSettings
{
    [SerializeField, Tooltip("Enable this if you want just white!")] bool _allWhite = false;
    [SerializeField] bool _invert = false;
    [SerializeField] bool _mask = false;
    [SerializeField] float _maskValue = 0.5f;
    [SerializeField, Range(0, 5), Tooltip("Add this to every point in the noise to make it more white")] float _addValue = 0;
    [SerializeField, Range(0, 15), Tooltip("How strong should this noise have as an effect?")] float _strength = 1.0f;

    [SerializeField, Range(0, 100000), Tooltip("Gives a different starting point for noise")] int _seed;
    [SerializeField, Tooltip("Offset from noise seed if desired")] Vector2 _offset;
    [SerializeField, Range(0.001f, 100000), Tooltip("Scale of Noise")] float _scale = 10;
    [SerializeField, Range(1, 10), Tooltip("Amount of noiselayers stacked on eachother (3 is good)")] int _octaves = 4;
    [SerializeField, Range(0, 1), Tooltip("Amount of increase in frequency of each octave")] float _persistance = 0.6f;
    [SerializeField, Range(1, 25), Tooltip("How much effect each octave should have")] float _lacunarity = 1.5f;

    [SerializeField, Range(-3, 3)] float _darken = 0.0f;
    [SerializeField] bool _useDarken = false;

    [SerializeField, Tooltip("Keep on GLOBAL! LOCAL is only used for testing purposes!")] Noise.NormalizeMode _normalizeMode = Noise.NormalizeMode.GLOBAL;

    public bool AllWhite                     { get { return _allWhite; }      private set { _allWhite = value; } }
    public bool Invert                       { get { return _invert; }        private set { _invert = value; } }
    public bool Mask                         { get { return _mask; } private set { _mask = value; } }
    public float MaskValue                   { get { return _maskValue; } private set { _maskValue = value; } }
    public float AddValue                    { get { return _addValue; }      private set { _addValue = value; } }
    public float Strength                    { get { return _strength; }      private set { _strength = value; } }

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
