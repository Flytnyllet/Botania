﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "Generation/Spawning/Biome")]
public class Biome : UpdatableData
{
    [SerializeField] Spawnable[] _spawnables;
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] NoiseSettingsData _offsetNoiseSettings;
    [SerializeField, Tooltip("Offset for offsetNoise per children")] Vector2 _offsetVectorForOffsetNoise;
    [SerializeField] GameObject _waterChunk;
    [SerializeField] float _waterHeight;

    public Spawnable[] Spawnables { get { return _spawnables; }  private set { _spawnables = value; } }

    public GameObject WaterChunk  { get { return _waterChunk; }  private set { _waterChunk = value; } }
    public float WaterHeight      { get { return _waterHeight; } private set { _waterHeight = value; } }

    public Biome(Biome biome)
    {
        this._spawnables = biome.Spawnables;
        this._spawnables = Spawnable.CopySpawnables(_spawnables);
        this._meshSettings = biome._meshSettings;
        this._offsetNoiseSettings = biome._offsetNoiseSettings;
        this._offsetVectorForOffsetNoise = biome._offsetVectorForOffsetNoise;

        this._waterChunk = biome._waterChunk;
        this._waterHeight = biome._waterHeight;
    }

    public void Setup(Vector2 center)
    {
        for (int i = 0; i < _spawnables.Length; i++)
        {
            _spawnables[i].Setup(null, _meshSettings.ChunkSize, _offsetNoiseSettings, center, _offsetVectorForOffsetNoise);
        }
    }
}
