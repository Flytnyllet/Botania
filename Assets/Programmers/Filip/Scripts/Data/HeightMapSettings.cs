using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HeightMapSettings", menuName = "Generation/HeightMapSettings")]
public class HeightMapSettings : UpdatableData
{
    [SerializeField] NoiseSettings _noiseSettings;

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

    public NoiseSettings NoiseSettings { get { return _noiseSettings; } private set { _noiseSettings = value; } }

    public float HeightMultiplier { get { return _heightMultiplier; } private set { _heightMultiplier = value; } }
    public AnimationCurve HeightCurve { get { return _heightCurve; } private set { _heightCurve = value; } }
    public bool UseFallOfMap { get { return _useFallOfMap; } private set { _useFallOfMap = value; } }
}
