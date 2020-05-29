using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "Generation/Spawning/Biome")]
public class Biome : UpdatableData
{
    [SerializeField] Spawnable[] _highLODSpawnable;
    [SerializeField] Spawnable[] _mediumLODSpawnable;
    [SerializeField] Spawnable[] _lowLODSpawnable;

    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] NoiseSettingsData _offsetNoiseSettings;
    [SerializeField, Tooltip("Offset for offsetNoise per children")] Vector2 _offsetVectorForOffsetNoise;
    [SerializeField] GameObject _waterChunk;
    [SerializeField] float _waterHeight;

    public Spawnable[] HighLODSpawnable   { get { return _highLODSpawnable; }   private set { _highLODSpawnable = value; } }
    public Spawnable[] MediumLODSpawnable { get { return _mediumLODSpawnable; } private set { _mediumLODSpawnable = value; } }
    public Spawnable[] LowLODSpawnable    { get { return _lowLODSpawnable; }    private set { _lowLODSpawnable = value; } }

    public GameObject WaterChunk  { get { return _waterChunk; }  private set { _waterChunk = value; } }
    public float WaterHeight      { get { return _waterHeight; } private set { _waterHeight = value; } }

    public Biome(Biome biome)
    {
        this._highLODSpawnable = biome.HighLODSpawnable;
        this._highLODSpawnable = Spawnable.CopySpawnables(_highLODSpawnable);
        this._mediumLODSpawnable = biome.MediumLODSpawnable;
        this._mediumLODSpawnable = Spawnable.CopySpawnables(_mediumLODSpawnable);
        this._lowLODSpawnable = biome.LowLODSpawnable;
        this._lowLODSpawnable = Spawnable.CopySpawnables(_lowLODSpawnable);
        this._meshSettings = biome._meshSettings;
        this._offsetNoiseSettings = biome._offsetNoiseSettings;
        this._offsetVectorForOffsetNoise = biome._offsetVectorForOffsetNoise;

        this._waterChunk = biome._waterChunk;
        this._waterHeight = biome._waterHeight;
    }

    public void Setup(Vector2 center, int LODIndex)
    {
        switch (LODIndex)
        {
            case (0):
                {
                    for (int i = 0; i < _highLODSpawnable.Length; i++)
                    {
                        _highLODSpawnable[i].Setup(null, _meshSettings.ChunkSize, _offsetNoiseSettings, center, _offsetVectorForOffsetNoise);
                    }
                    break;
                }
            case (1):
                {
                    for (int i = 0; i < _mediumLODSpawnable.Length; i++)
                    {
                        _mediumLODSpawnable[i].Setup(null, _meshSettings.ChunkSize, _offsetNoiseSettings, center, _offsetVectorForOffsetNoise);
                    }
                    break;
                }
            case (2):
                {
                    for (int i = 0; i < _lowLODSpawnable.Length; i++)
                    {
                        _lowLODSpawnable[i].Setup(null, _meshSettings.ChunkSize, _offsetNoiseSettings, center, _offsetVectorForOffsetNoise);
                    }
                    break;
                }
            default: Debug.LogError("There is no LOD index that is " + LODIndex); break;
        }  
    }
}
