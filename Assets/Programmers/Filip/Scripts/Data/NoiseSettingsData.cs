using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NoiseSettingsData", menuName = "Generation/NoiseSettingsData")]
public class NoiseSettingsData : UpdatableData
{
    [SerializeField] NoiseSettings _noiseSettings;

    public NoiseSettings NoiseSettings { get { return _noiseSettings; } private set { _noiseSettings = value; } }
}
