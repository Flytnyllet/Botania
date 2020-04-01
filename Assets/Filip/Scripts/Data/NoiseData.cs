using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NoiseData", menuName = "Generation/NoiseData")]
public class NoiseData : UpdatableData
{
    [SerializeField, Range(0, 100000), Tooltip("Gives a different starting point for noise")] int _seed;
    [SerializeField, Tooltip("Offset from noise seed if desired")] Vector2 _offset;
    [SerializeField, Range(0.001f, 100), Tooltip("Scale of Noise")] float _noiseScale;
    [SerializeField, Range(1, 10), Tooltip("Amount of noiselayers stacked on eachother (3 is good)")] int _octaves;
    [SerializeField, Range(0, 1), Tooltip("Amount of increase in frequency of each octave")] float _persistance;
    [SerializeField, Range(1, 25), Tooltip("How much effect each octave should have")] float _lacunarity;

    [SerializeField, Tooltip("Keep on GLOBAL! LOCAL is only used for testing purposes!")] Noise.NormalizeMode _normalizeMode;

    public int GetSeed() { return _seed; }
    public Vector2 GetOffset() { return _offset; }
    public float GetNoiseScale() { return _noiseScale; }
    public int GetOctaves() { return _octaves; }
    public float GetPersistance() { return _persistance; }
    public float GetLacunarity() { return _lacunarity; }
    public Noise.NormalizeMode GetNormalizeMode() { return _normalizeMode; }
}
