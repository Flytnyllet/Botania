using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawnable", menuName = "Generation/Spawning/Spawnable")]
public class Spawnable : UpdatableData
{
    [Header("Affect by parent")]
    [SerializeField, Tooltip("Should almost always be set to MULTIPLY (area within parent)")] NoiseMergeType _noiseMergeType;


    [Header("Noise settings")]

    [SerializeField, Tooltip("Settings to create the noise the prefab should spawn after")] NoiseSettingsData _noiseSettingsData;


    [Header("General Settings")]
    
    [SerializeField, Range(0, 1), Tooltip("At which point in the noise gradient should object start spawning? Low = smooth edges, high = sharp af")] float _noiseStartPoint;
    [SerializeField, Range(0, 2), Tooltip("How thick the area of spawn should be")] float _thickness = 0.75f;
    [SerializeField, Range(1, 80), Tooltip("Uniform spread amount (use for general spreading and not for fine tuning) (low = low spread, high = high spread)")] int _uniformSpreadAmount = 1;
    [SerializeField, Range(0, 1), Tooltip("How much random spawn spread there should be")] float _randomSpread = 0.5f;
    [SerializeField, Range(0, 4), Tooltip("How much should the spawning avert from the grid it is based on? (high values might cause clipping!!!)")] float _offsetAmount = 0.75f;


    [Header("Height Spawn Settings")]

    [SerializeField, Range(0, 100), Tooltip("At which height should objects start spawning? (soft amount)")] float _softMinHeight = 0;
    [SerializeField, Range(0, 100), Tooltip("At which height should objects start spawning? (hard amount)")] float _hardMinHeight = 0;
    [SerializeField, Range(0, 100), Tooltip("At which height should objects stop spawning? (soft amount)")] float _softMaxHeight = 0;
    [SerializeField, Range(0, 100), Tooltip("At which height should objects stop spawning? (hard amount)")] float _hardMaxHeight = 0;


    [Header("Drop")]

    [SerializeField, Tooltip("Which object should spawn?")] GameObject _prefab;
    [SerializeField, Tooltip("Sub areas within this objects noise in which new objects can spawn")] Spawnable[] _subSpawners;

    float[,] _noise;
    float[,] _offsetNoise;
    float[,] _spreadNoise;

    public NoiseMergeType NoiseMergeType              { get { return _noiseMergeType; }      private set { _noiseMergeType = value; } }
    public GameObject Prefab                          { get { return _prefab; }              private set { _prefab = value; } }
    public NoiseSettingsData NoiseSettingsData        { get { return _noiseSettingsData; }   private set { _noiseSettingsData = value; } }
    public Spawnable[] SubSpawners                    { get { return _subSpawners; }         private set { _subSpawners = value; } }
    public float NoiseStartPoint                      { get { return _noiseStartPoint; }     private set { _noiseStartPoint = value; } }
    public float Thickness                            { get { return _thickness; }           private set { _thickness = value; } }
    public int UniformSpreadAmount                    { get { return _uniformSpreadAmount; } private set { _uniformSpreadAmount = value; } }
    public float RandomSpread                         { get { return _randomSpread; }        private set { _randomSpread = value; } }
    public float OffsetAmount                         { get { return _offsetAmount; }        private set { _offsetAmount = value; } }
    public float SoftMinHeight                        { get { return _softMinHeight; }       private set { _softMinHeight = value; } }
    public float HardMinHeight                        { get { return _hardMinHeight; }       private set { _hardMinHeight = value; } }
    public float SoftMaxHeight                        { get { return _softMaxHeight; }       private set { _softMaxHeight = value; } }
    public float HardMaxHeight                        { get { return _hardMaxHeight; }       private set { _hardMaxHeight = value; } }

    public float[,] GetNoise                          { get { return _noise; }               private set { _noise = value; } }
    public float[,] OffsetNoise                       { get { return _offsetNoise; }         private set { _offsetNoise = value; } }
    public float[,] SpreadNoise                       { get { return _spreadNoise; }         private set { _spreadNoise = value; } }

    // Used to calculate all the different noises for every spawable
    public void Setup(float[,] parentNoise, int chunkSize, NoiseSettingsData offsetNoiseSettings, Vector2 center, Vector2 offsetNoiseOffset)
    {
        if (parentNoise != null)
            _noise = Noise.MergeNoise(chunkSize, chunkSize, _noiseSettingsData.NoiseSettingsDataMerge, parentNoise, _noiseMergeType, center);
        else
            _noise = Noise.GenerateNoiseMap(chunkSize, chunkSize, _noiseSettingsData.NoiseSettingsDataMerge, center);

        _offsetNoise = Noise.GenerateNoiseMap(chunkSize, chunkSize, offsetNoiseSettings.NoiseSettingsDataMerge, center + offsetNoiseOffset);
        _spreadNoise = Noise.GenerateNoiseMap(chunkSize, chunkSize, offsetNoiseSettings.NoiseSettingsDataMerge, center + offsetNoiseOffset * 2);

        for (int i = 0; i < _subSpawners.Length; i++)
        {
            _subSpawners[i].Setup(_noise, chunkSize, offsetNoiseSettings, center, offsetNoiseOffset * 2);
        }
    }
}