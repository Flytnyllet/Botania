using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "Generation/Spawning/Biome")]
public class Biome : UpdatableData
{
    [SerializeField] Spawnable[] _spawnables;
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] HeightMapSettings _offsetNoise;

    public Spawnable[] Spawnables { get { return _spawnables; } private set { _spawnables = value; } }
    public HeightMapSettings OffsetNoise { get { return _offsetNoise; } private set { _offsetNoise = value; } }

    public void Setup()
    {
        for (int i = 0; i < _spawnables.Length; i++)
        {
            _spawnables[i].Setup(null, _meshSettings.ChunkSize);
        }
    }
}
