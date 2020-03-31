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
        COLOR_MAP,
        MESH
    }

    public static readonly int MAP_CHUNK_SIZE = 241;

    [Header("General Settings")]

    [SerializeField] DrawMode _drawMode;
    [SerializeField, Tooltip("Should the terrainmap update in the editor?")] bool _autoUpdate;
    [SerializeField] TerrainType[] _regions;

    [Header("Noise Settings")]

    [SerializeField, Range(0, 100000), Tooltip("Gives a different starting point for noise")] int _seed;
    [SerializeField, Tooltip("Offset from noise seed if desired")] Vector2 _offset;
    [SerializeField, Range(0.001f, 100), Tooltip("Scale of Noise")] float _noiseScale;
    [SerializeField, Range(1, 10), Tooltip("Amount of noiselayers stacked on eachother (3 is good)")] int _octaves;
    [SerializeField, Range(0, 1), Tooltip("Amount of increase in frequency of each octave")] float _persistance;
    [SerializeField, Range(1, 25), Tooltip("How much effect each octave should have")] float _lacunarity;

    [Header("Mesh Settings")]

    [SerializeField, Range(-100, 100)] float _meshHeightMultiplier;
    [SerializeField] AnimationCurve _meshHeightCurve;
    [SerializeField, Range(0, 6)] int _editorPreviewLevelOfDetail;

    Queue<MapThreadInfo<MapData>> _mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public bool DoAutoUpdate() { return _autoUpdate; }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        //Ändra senare kan vara slow
        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (_drawMode == DrawMode.NOISE_MAP)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        else if (_drawMode == DrawMode.COLOR_MAP)
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
        else if (_drawMode == DrawMode.MESH)
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, _meshHeightMultiplier, _meshHeightCurve, _editorPreviewLevelOfDetail), TextureGenerator.TextureFromColorMap(mapData.colorMap, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
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
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, _meshHeightMultiplier, _meshHeightCurve, levelOfDetail);
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
        float[,] noiseMap = Noise.GenerateNoiseMap(MAP_CHUNK_SIZE, MAP_CHUNK_SIZE, _seed, _noiseScale, _octaves, _persistance, _lacunarity, centre + _offset);

        //Generate colors from regions based on height in terrain
        Color[] colorMap = new Color[MAP_CHUNK_SIZE * MAP_CHUNK_SIZE];
        for (int y = 0; y < MAP_CHUNK_SIZE; y++)
        {
            for (int x = 0; x < MAP_CHUNK_SIZE; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < _regions.Length; i++)
                {
                    if (currentHeight <= _regions[i].height)
                    {
                        colorMap[y * MAP_CHUNK_SIZE + x] = _regions[i].color;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
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

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}