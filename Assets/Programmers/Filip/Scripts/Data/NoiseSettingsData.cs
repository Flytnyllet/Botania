using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NoiseSettingsData", menuName = "Generation/NoiseSettingsData")]
public class NoiseSettingsData : UpdatableData
{
    [SerializeField] NoiseSettingsDataMerge[] _noiseSettings;

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