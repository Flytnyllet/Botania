using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HeightMapSettings", menuName = "Generation/HeightMapSettings")]
public class HeightMapSettings : UpdatableData
{
    [SerializeField] NoiseSettingsData _noiseSettingsData;

    [SerializeField, Range(-100, 100)] float _heightMultiplier;
    [SerializeField] AnimationCurve _heightCurve;
    [SerializeField] bool _useFallOfMap;


    public float MinHeight
    {
        get { return _heightMultiplier * _heightCurve.Evaluate(0); }
    }

    public float MaxHeight
    {
        get { return _heightMultiplier * _heightCurve.Evaluate(1); }
    }

    public NoiseSettingsData NoiseSettingsData { get { return _noiseSettingsData; } private set { _noiseSettingsData = value; } }

    public float HeightMultiplier { get { return _heightMultiplier; } private set { _heightMultiplier = value; } }
    public AnimationCurve HeightCurve { get { return _heightCurve; } private set { _heightCurve = value; } }
    public bool UseFallOfMap { get { return _useFallOfMap; } private set { _useFallOfMap = value; } }
}
