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

    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] HeightMapSettings _heightMapSettings;
    [SerializeField] TextureData _textureData;
    [SerializeField] Material _terrainMaterial;

    [Header("Mesh Settings")]

    [SerializeField, Range(0, MeshSettings.NUMBER_SUPPORTED_LODS - 1)] int _editorPreviewLevelOfDetail;

    float[,] _fallofMap;

    Queue<MapThreadInfo<HeightMap>> _heightMapThreadInfoQueue = new Queue<MapThreadInfo<HeightMap>>();
    Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    static int _mapChunkSize;

    private void Start()
    {
        _textureData.ApplyToMaterial(_terrainMaterial);
        _textureData.UpdateMeshHeights(_terrainMaterial, _heightMapSettings.MinHeight, _heightMapSettings.MaxHeight);
    }

    public int ChunkSize()
    {
        return _meshSettings.ChunkSize;
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
        _mapChunkSize = _meshSettings.ChunkSize;

        if (_meshSettings != null)
        {
            _meshSettings.OnValuesUpdated -= OnValuesUpdated;
            _meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (_heightMapSettings != null)
        {
            _heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            _heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (_textureData != null)
        {
            _textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            _textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }

    public bool DoAutoUpdate() { return _autoUpdate; }
    
    public float GetScale() { return _meshSettings.MeshScale; }

    public void DrawMapInEditor()
    {
        _textureData.UpdateMeshHeights(_terrainMaterial, _heightMapSettings.MinHeight, _heightMapSettings.MaxHeight);

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(_meshSettings.ChunkSize + 2, _meshSettings.ChunkSize + 2, _heightMapSettings, Vector2.zero);
        //Ändra senare kan vara slow
        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (_drawMode == DrawMode.NOISE_MAP)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap.heightMap));
        else if (_drawMode == DrawMode.MESH)
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.heightMap, _meshSettings, _editorPreviewLevelOfDetail));
        else if (_drawMode == DrawMode.FALL_OF_MAP)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallofGenerator.GenerateFallofMap(_meshSettings.ChunkSize)));
    }

    public void RequestHeightMap(Vector2 centre, Action<HeightMap> callback)
    {
        ThreadStart threadStart = delegate { HeightMapThread(centre, callback); };

        new Thread(threadStart).Start();
    }

    void HeightMapThread(Vector2 centre, Action<HeightMap> callback)
    {
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(_meshSettings.ChunkSize + 2, _meshSettings.ChunkSize + 2, _heightMapSettings, centre);

        //If heightMaphreadqueue is being accessed by another thread, WAIT! :D 
        lock (_heightMapThreadInfoQueue)
        {
            _heightMapThreadInfoQueue.Enqueue(new MapThreadInfo<HeightMap>(callback, heightMap));
        }
    }

    public void RequestMeshData(HeightMap heightMap, int levelOfDetail, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate { MeshDataThread(heightMap, levelOfDetail, callback); };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(HeightMap heightMap, int levelOfDetail, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.heightMap, _meshSettings, levelOfDetail);
        lock (_meshDataThreadInfoQueue)
        {
            _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {
        if (_heightMapThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < _heightMapThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<HeightMap> threadInfo = _heightMapThreadInfoQueue.Dequeue();
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