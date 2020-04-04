using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawnable", menuName = "Generation/Spawning/Spawnable")]
public class Spawnable : UpdatableData
{
    [Header("Affect by parent")]
    [SerializeField] bool _baseOnParentNoise;
    [SerializeField] NoiseMergeType _noiseMergeType;

    [Header("General Settings")]

    [SerializeField] GameObject _prefab;
    [SerializeField] NoiseSettings _noiseSettings;
    [SerializeField] Spawnable[] _subSpawners;

    float[,] _noise;

    public bool BaseOnParentNoise { get { return _baseOnParentNoise; } private set { _baseOnParentNoise = value; } }
    public NoiseMergeType NoiseMergeType { get { return _noiseMergeType; } private set { _noiseMergeType = value; } }
    public GameObject Prefab { get { return _prefab; } private set { _prefab = value; } }
    public NoiseSettings NoiseSettings { get { return _noiseSettings; } private set { _noiseSettings = value; } }
    public float[,] GetNoise { get { return _noise; } private set { _noise = value; } }
    public Spawnable[] SubSpawners { get { return _subSpawners; } private set { _subSpawners = value; } }

    public void Setup(float[,] parentNoise, int chunkSize, Vector2 center)
    {
        if (parentNoise != null && _baseOnParentNoise)
            _noise = Noise.MergeNoise(chunkSize, chunkSize, _noiseSettings, parentNoise, _noiseMergeType, center);
        else
            _noise = Noise.GenerateNoiseMap(chunkSize, chunkSize, _noiseSettings, center);

        for (int i = 0; i < _subSpawners.Length; i++)
        {
            _subSpawners[i].Setup(_noise, chunkSize, center);
        }
    }
}