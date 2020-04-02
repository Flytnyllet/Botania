using System.Collections;
using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NOISE_MAP,
        MESH,
        FALL_OF_MAP
    }

    [Header("General Settings")]

    [SerializeField] DrawMode _drawMode;
    [SerializeField, Tooltip("Should the terrainmap update in the editor?")] bool _autoUpdate;
    [SerializeField, Tooltip("Keep on GLOBAL! LOCAL is only used for testing purposes!")] Noise.NormalizeMode _normalizeMode;

    [SerializeField] TerrainData _terrainData;
    [SerializeField] NoiseData _noiseData;
    [SerializeField] TextureData _textureData;
    [SerializeField] Material _terrainMaterial;
    [SerializeField, Range(0, MeshGenerator.NUMBER_SUPPORTED_CHUNK_SIZES - 1)] int _chunkSizeIndex;

    [Header("Mesh Settings")]

    [SerializeField, Range(0, MeshGenerator.NUMBER_SUPPORTED_LODS - 1)] int _editorPreviewLevelOfDetail;

    float[,] _fallofMap;

    Queue<MapThreadInfo<MapData>> _mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    static int _mapChunkSize;

    public static int GetChunkSize()
    {
        return _mapChunkSize;
    }

    private void Awake()
    {
        _mapChunkSize = MeshGenerator.SUPPORTED_CHUNK_SIZES[_chunkSizeIndex] - 1;

        _textureData.ApplyToMaterial(_terrainMaterial);
        _textureData.UpdateMeshHeights(_terrainMaterial, _terrainData.MinHeight, _terrainData.MaxHeight);
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
            DrawMapInEditor();
    }

    void OnTextureValuesUpdated()
    {
        _textureData.ApplyToMaterial(_terrainMaterial);
    }

    //Primaraly used to init things in edit mode!
    private void OnValidate()
    {
        _mapChunkSize = MeshGenerator.SUPPORTED_CHUNK_SIZES[_chunkSizeIndex] - 1;

        if (_terrainData != null)
        {
            _terrainData.OnValuesUpdated -= OnValuesUpdated;
            _terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (_noiseData != null)
        {
            _noiseData.OnValuesUpdated -= OnValuesUpdated;
            _noiseData.OnValuesUpdated += OnValuesUpdated;
        }
        if (_textureData != null)
        {
            _textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            _textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }

    public bool DoAutoUpdate() { return _autoUpdate; }
    
    public float GetScale() { return _terrainData.GetUniformScale(); }

    public void DrawMapInEditor()
    {
        _textureData.UpdateMeshHeights(_terrainMaterial, _terrainData.MinHeight, _terrainData.MaxHeight);

        MapData mapData = GenerateMapData(Vector2.zero);
        //Ändra senare kan vara slow
        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (_drawMode == DrawMode.NOISE_MAP)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        else if (_drawMode == DrawMode.MESH)
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, _terrainData.GetMeshHeightMultiplier(), _terrainData.GetMeshHeightCurve(), _editorPreviewLevelOfDetail));
        else if (_drawMode == DrawMode.FALL_OF_MAP)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallofGenerator.GenerateFallofMap(GetChunkSize())));
    }

    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate { MapDataThread(centre, callback); };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);

        //If mapdatahreadqueue is being accessed by another thread, WAIT! :D 
        lock (_mapDataThreadInfoQueue)
        {
            _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int levelOfDetail, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate { MeshDataThread(mapData, levelOfDetail, callback); };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int levelOfDetail, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, _terrainData.GetMeshHeightMultiplier(), _terrainData.GetMeshHeightCurve(), levelOfDetail);
        lock (_meshDataThreadInfoQueue)
        {
            _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        if (_mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < _mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = _mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (_meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < _meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = _meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    private MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(_normalizeMode, GetChunkSize() + 2, GetChunkSize() + 2, _noiseData.GetSeed(), _noiseData.GetNoiseScale(), _noiseData.GetOctaves(), _noiseData.GetPersistance(), _noiseData.GetLacunarity(), centre + _noiseData.GetOffset());

        if (_terrainData.GetUseFallofMap())
        {
            if (_fallofMap == null)
                _fallofMap = FallofGenerator.GenerateFallofMap(GetChunkSize() + 2);

            for (int y = 0; y < GetChunkSize() + 2; y++)
            {
                for (int x = 0; x < GetChunkSize() + 2; x++)
                {
                    if (_terrainData.GetUseFallofMap())
                        noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y] - _fallofMap[x, y], 0, 1);
                }
            }
        }

        return new MapData(noiseMap);
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo (Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}


public struct MapData
{
    public readonly float[,] heightMap;

    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}