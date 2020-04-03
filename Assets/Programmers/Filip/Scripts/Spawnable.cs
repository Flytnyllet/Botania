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

    [Header("Spawner Settings")]


    NoiseSettings _parentNoise;
    float[,] _noise;
    int _chunkSize;

    public bool BaseOnParentNoise { get { return _baseOnParentNoise; } private set { _baseOnParentNoise = value; } }
    public NoiseMergeType NoiseMergeType { get { return _noiseMergeType; } private set { _noiseMergeType = value; } }
    public GameObject Prefab { get { return _prefab; } private set { _prefab = value; } }
    public NoiseSettings NoiseSettings { get { return _noiseSettings; } private set { _noiseSettings = value; } }
    public Spawnable[] SubSpawners { get { return _subSpawners; } private set { _subSpawners = value; } }

    public void Setup(NoiseSettings parentNoise, int chunkSize)
    {
        this._chunkSize = chunkSize;

        if (parentNoise != null)
            this._parentNoise = parentNoise;

        for (int i = 0; i < _subSpawners.Length; i++)
        {
            _subSpawners[i].Setup(_noiseSettings, chunkSize);
        }

    }

    public float[,] GetNoise(Vector2 center)
    {
        if (_baseOnParentNoise && _parentNoise != null)
            return Noise.MergeNoise(_chunkSize, _chunkSize, _noiseSettings, _parentNoise, _noiseMergeType, center);
        else
        {
            return Noise.GenerateNoiseMap(_chunkSize, _chunkSize, _noiseSettings, center);
        }
    }
}