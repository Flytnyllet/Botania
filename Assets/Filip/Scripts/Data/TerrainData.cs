using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TerrainData", menuName = "Generation/TerrainData")]
public class TerrainData : UpdatableData
{
    [SerializeField] float _uniformScale = 1f;

    [SerializeField, Range(-100, 100)] float _meshHeightMultiplier;
    [SerializeField] AnimationCurve _meshHeightCurve;
    [SerializeField] bool _useFallOfMap;


    public float MinHeight
    {
        get { return _uniformScale * _meshHeightMultiplier * _meshHeightCurve.Evaluate(0); }
    }

    public float MaxHeight
    {
        get { return _uniformScale * _meshHeightMultiplier * _meshHeightCurve.Evaluate(1); }
    }

    public float GetUniformScale() { return _uniformScale; }
    public float GetMeshHeightMultiplier() { return _meshHeightMultiplier; }
    public AnimationCurve GetMeshHeightCurve() { return _meshHeightCurve; }
    public bool GetUseFallofMap() { return _useFallOfMap; }
}
