using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "Generation/Spawning/Biome")]
public class Biome : UpdatableData
{
    [SerializeField] Spawnable[] _spawnables;
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] NoiseSettingsData _offsetNoiseSettings;
    [SerializeField, Tooltip("Offset for offsetNoise per children")] Vector2 _offsetVectorForOffsetNoise;

    bool[,] _occupiedGrid; 

    public Spawnable[] Spawnables { get { return _spawnables; } private set { _spawnables = value; } }
    public bool[,] OccupiedGrid { get { return _occupiedGrid; } private set { _occupiedGrid = value; } }

    public void Setup(Vector2 center)
    {
        _occupiedGrid = new bool[_meshSettings.ChunkSize - 1, _meshSettings.ChunkSize - 1];

        for (int i = 0; i < _spawnables.Length; i++)
        {
            _spawnables[i].Setup(null, _meshSettings.ChunkSize, _offsetNoiseSettings, center, _offsetVectorForOffsetNoise);
        }
    }

    //Returns true if the object can fit in chunk where it is trying to fit
    public bool CanObjectSpawn(int x, int y, int size)
    {
        int maxX = x + size < _meshSettings.ChunkSize - 1 ? x + size : _meshSettings.ChunkSize - 1;
        int maxY = y + size < _meshSettings.ChunkSize - 1 ? y + size : _meshSettings.ChunkSize - 1;

        for (int checkX = x; checkX < maxX; checkX++)
        {
            for (int checkY = y; checkY < maxY; checkY++)
            {
                if (_occupiedGrid[checkX, checkY])
                    return false;
            }
        }

        return true;
    }

    //Tells other objects this spot it taken lol
    public void OccupyWithObject(int x, int y, int size)
    {
        int maxX = x + size < _meshSettings.ChunkSize - 1 ? x + size : _meshSettings.ChunkSize - 1;
        int maxY = y + size < _meshSettings.ChunkSize - 1 ? y + size : _meshSettings.ChunkSize - 1;

        for (int checkX = x; checkX < maxX; checkX++)
        {
            for (int checkY = y; checkY < maxY; checkY++)
            {
                _occupiedGrid[checkX, checkY] = true;
            }
        }
    }
}
