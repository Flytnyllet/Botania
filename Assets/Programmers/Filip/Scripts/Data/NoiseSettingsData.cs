using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NoiseSettingsData", menuName = "Generation/NoiseSettingsData")]
public class NoiseSettingsData : UpdatableData
{
    [SerializeField] float _clampMin = float.MinValue;
    [SerializeField] float _clampMax = float.MaxValue;
    [SerializeField] NoiseSettingsDataMerge[] _noiseSettings;

    public float ClampMin { get { return _clampMin; } private set { _clampMin = value; } }
    public float ClampMax { get { return _clampMax; } private set { _clampMax = value; } }
    public NoiseSettingsDataMerge[] NoiseSettingsDataMerge { get { return _noiseSettings; } private set { _noiseSettings = value; } }
}

[System.Serializable]
public class NoiseSettingsDataMerge
{
    [SerializeField] NoiseMergeType _noiseMergeType;
    [SerializeField] NoiseSettings _noiseSettings;

    public NoiseMergeType MoiseMergeType { get { return _noiseMergeType; } private set { _noiseMergeType = value; } }
    public NoiseSettings NoiseSettings { get { return _noiseSettings; } private set { _noiseSettings = value; } }
}