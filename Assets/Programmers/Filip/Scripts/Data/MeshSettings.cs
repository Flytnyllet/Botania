using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MeshSettings", menuName = "Generation/MeshSettings")]
public class MeshSettings : UpdatableData
{
    public const int NUMBER_SUPPORTED_LODS = 5;
    public const int NUMBER_SUPPORTED_CHUNK_SIZES = 9;
    public static readonly int[] SUPPORTED_CHUNK_SIZES = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };


    [SerializeField] float _meshScale = 1f;
    [SerializeField, Range(0, NUMBER_SUPPORTED_CHUNK_SIZES - 1)] int _chunkSizeIndex;

    public float MeshScale { get { return _meshScale; } private set { _meshScale = value; } }
    public int ChunkSizeIndex { get { return _chunkSizeIndex; } set { _chunkSizeIndex = value; } }

    public int ChunkSize
    {
        get { return SUPPORTED_CHUNK_SIZES[ChunkSizeIndex] + 3; }
    }

    public float MeshWorldSize
    {
        get
        {
            return (NumVertsPerLine - 3) * _meshScale;
        }
    }

    public int NumVertsPerLine
    {
        get { return SUPPORTED_CHUNK_SIZES[ChunkSizeIndex] + 5; }
    }
}
