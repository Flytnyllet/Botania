using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawnable", menuName = "Generation/Spawning/Spawnable")]
public class Spawnable : UpdatableData
{
    [Header("Affect by parent")]
    [SerializeField, Tooltip("Should almost always be set to MULTIPLY (area within parent)")] NoiseMergeType _noiseMergeType;


    [Header("Noise settings")]

    [SerializeField, Tooltip("Settings to create the noise the prefab should spawn after")] NoiseSettings _noiseSettings;


    [Header("General Settings")]
    
    [SerializeField, Range(0, 1), Tooltip("At which point in the noise gradient should object start spawning? Low = smooth edges, high = sharp af")] float _noiseStartPoint;
    [SerializeField, Range(0, 2), Tooltip("How thick the area of spawn should be")] float _thickness = 0.75f;
    [SerializeField, Range(1, 80), Tooltip("Uniform spread amount (use for general spreading and not for fine tuning) (low = low spread, high = high spread)")] int _uniformSpreadAmount = 1;
    [SerializeField, Range(0, 1), Tooltip("How much random spawn spread there should be")] float _randomSpread = 0.5f;
    [SerializeField, Range(0, 4), Tooltip("How much should the spawning avert from the grid it is based on? (high values might cause clipping!!!)")] float _offsetAmount = 0.75f;


    [Header("Drop")]

    [SerializeField, Tooltip("Which object should spawn?")] GameObject _prefab;
    [SerializeField, Tooltip("Sub areas within this objects noise in which new objects can spawn")] Spawnable[] _subSpawners;

    float[,] _noise;

    public NoiseMergeType NoiseMergeType { get { return _noiseMergeType; }      private set { _noiseMergeType = value; } }
    public GameObject Prefab             { get { return _prefab; }              private set { _prefab = value; } }
    public NoiseSettings NoiseSettings   { get { return _noiseSettings; }       private set { _noiseSettings = value; } }
    public float[,] GetNoise             { get { return _noise; }               private set { _noise = value; } }
    public Spawnable[] SubSpawners       { get { return _subSpawners; }         private set { _subSpawners = value; } }
    public float NoiseStartPoint         { get { return _noiseStartPoint; }     private set { _noiseStartPoint = value; } }
    public float Thickness               { get { return _thickness; }           private set { _thickness = value; } }
    public int UniformSpreadAmount       { get { return _uniformSpreadAmount; } private set { _uniformSpreadAmount = value; } }
    public float RandomSpread            { get { return _randomSpread; }        private set { _randomSpread = value; } }
    public float OffsetAmount            { get { return _offsetAmount; }        private set { _offsetAmount = value; } }

    // Used to calculate all the different noises for every spawable
    public void Setup(float[,] parentNoise, int chunkSize, Vector2 center)
    {
        if (parentNoise != null)
            _noise = Noise.MergeNoise(chunkSize, chunkSize, _noiseSettings, parentNoise, _noiseMergeType, center);
        else
            _noise = Noise.GenerateNoiseMap(chunkSize, chunkSize, _noiseSettings, center);

        for (int i = 0; i < _subSpawners.Length; i++)
        {
            _subSpawners[i].Setup(_noise, chunkSize, center);
        }
    }
}