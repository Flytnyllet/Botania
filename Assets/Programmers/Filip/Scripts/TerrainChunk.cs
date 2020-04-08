using UnityEngine;
using System.Collections.Generic;

public class TerrainChunk
{
    static readonly float COLLIDER_GENERATION_DISTANCE_THRESHOLD = 5;

    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 Coord { get; private set; }

    GameObject _meshObject;
    Vector2 _sampleCenter;
    Bounds _bounds;

    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;
    MeshCollider _meshCollider;

    LODInfo[] _detailLevels;
    LODMesh[] _lodMeshes;
    int _colliderLODIndex;

    HeightMap _heightMap;
    bool _heightMapReceived;
    int _previousLODIndex = -1;
    bool _hasSetCollider = false;
    float _maxViewDistance;

    HeightMapSettings _heightMapSettings;
    MeshSettings _meshSettings;
    Transform _viewer;

    SpawnInfoRequester _spawnInfoRequester;
    Biome _biome;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material, Biome biome)
    {
        this._biome = biome;
        this.Coord = coord;
        this._detailLevels = detailLevels;
        this._colliderLODIndex = colliderLODIndex;
        this._heightMapSettings = heightMapSettings;
        this._meshSettings = meshSettings;
        this._viewer = viewer;

        _spawnInfoRequester = new SpawnInfoRequester();

        _sampleCenter = coord * meshSettings.MeshWorldSize / meshSettings.MeshScale;
        Vector2 position = coord * meshSettings.MeshWorldSize;
        _bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);

        _meshObject = new GameObject("Terrain Chunk");
        _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshFilter = _meshObject.AddComponent<MeshFilter>();
        _meshCollider = _meshObject.AddComponent<MeshCollider>();
        _meshRenderer.material = material;

        _meshObject.transform.position = new Vector3(position.x, 0, position.y);
        _meshObject.transform.parent = parent;
        SetVisable(false);

        _lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < _detailLevels.Length; i++)
        {
            _lodMeshes[i] = new LODMesh(_detailLevels[i].levelOfDetail);
            _lodMeshes[i]._updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
                _lodMeshes[i]._updateCallback += UpdateCollisionMesh;
        }

        _spawnInfoRequester._updateCallback += UpdateTerrainChunk;

        _maxViewDistance = _detailLevels[_detailLevels.Length - 1].visableDstThreshold;
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine, _heightMapSettings, _sampleCenter), OnHeightMapReceived);
    }

    void OnHeightMapReceived(object heightMapObject)
    {
        this._heightMap = (HeightMap)heightMapObject;
        _heightMapReceived = true;

        UpdateTerrainChunk();
    }

    Vector2 ViewerPosition { get { return new Vector2(_viewer.position.x, _viewer.position.z); } }

    public void UpdateTerrainChunk()
    {
        if (_heightMapReceived)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));

            bool wasVisible = IsVisable();
            bool visable = viewerDstFromNearestEdge <= _maxViewDistance;

            if (visable)
            {
                int lodIndex = 0;

                for (int i = 0; i < _detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > _detailLevels[i].visableDstThreshold)
                        lodIndex = i + 1;
                    else
                        break;
                }

                if (lodIndex != _previousLODIndex)
                {
                    LODMesh lodMesh = _lodMeshes[lodIndex];
                    if (lodMesh.HasMesh)
                    {
                        _previousLODIndex = lodIndex;
                        _meshFilter.mesh = lodMesh.Mesh;
                    }
                    else if (!lodMesh.HasRequestedMesh)
                        lodMesh.RequestMesh(_heightMap, _meshSettings);
                }

                if (_lodMeshes[lodIndex].HasMesh && !_spawnInfoRequester.IsSet && lodIndex == 0) //The mesh is spawned but no prefabs on it (it's also the highest detail level)
                {
                    if (_spawnInfoRequester.HasSpawnInfo) //The requested spawninfo has arrived! Hurray!
                    {
                        _spawnInfoRequester.IsSet = true;
                        PrefabSpawner.SpawnSpawnInfo(_spawnInfoRequester.SpawnInfo, _meshFilter.transform);
                    }
                    else if (!_spawnInfoRequester.HasRequestedSpawnInfo) //Request spawninfo!
                        _spawnInfoRequester.RequestSpawnInfo(_biome, _heightMap, _lodMeshes[lodIndex].MeshData, _meshSettings, new Vector2(_sampleCenter.x, -_sampleCenter.y));
                }

            }

            if (wasVisible != visable)
            {
                SetVisable(visable);
                if (onVisibilityChanged != null)
                    onVisibilityChanged(this, visable);
            }
        }

    }

    public void UpdateCollisionMesh()
    {
        if (!_hasSetCollider)
        {
            float sqrDistanceFromViewerToEdge = _bounds.SqrDistance(ViewerPosition);

            if (sqrDistanceFromViewerToEdge < _detailLevels[_colliderLODIndex].sqrVisibleDistanceThreshold)
            {
                if (!_lodMeshes[_colliderLODIndex].HasRequestedMesh)
                    _lodMeshes[_colliderLODIndex].RequestMesh(_heightMap, _meshSettings);
            }

            if (sqrDistanceFromViewerToEdge < COLLIDER_GENERATION_DISTANCE_THRESHOLD * COLLIDER_GENERATION_DISTANCE_THRESHOLD)
            {
                if (_lodMeshes[_colliderLODIndex].HasMesh)
                {
                    _meshCollider.sharedMesh = _lodMeshes[_colliderLODIndex].Mesh;
                    _meshCollider.sharedMesh = _lodMeshes[_colliderLODIndex].Mesh;
                    _hasSetCollider = true;
                }
            }
        }
    }

    public void SetVisable(bool visable)
    {
        _meshObject.SetActive(visable);
    }

    public bool IsVisable()
    {
        return _meshObject.activeSelf;
    }
}

class LODMesh
{
    public Mesh Mesh { get; private set; }
    public MeshData MeshData { get; private set; }
    public bool HasRequestedMesh { get; private set; }
    public bool HasMesh { get; private set; }
    int _levelOfDetail;

    public event System.Action _updateCallback;

    public LODMesh(int levelOfDetail)
    {
        this._levelOfDetail = levelOfDetail;
    }

    void OnMeshDataReceived(object meshData)
    {
        MeshData = (MeshData)meshData;
        Mesh = ((MeshData)meshData).CreateMesh();
        HasMesh = true;

        _updateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        HasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.heightMap, meshSettings, _levelOfDetail), OnMeshDataReceived);
    }
}

class SpawnInfoRequester
{
    public bool HasRequestedSpawnInfo { get; private set; }
    public bool HasSpawnInfo { get; private set; }
    public bool IsSet { get; set; }

    public event System.Action _updateCallback;

    public List<SpawnInfo> SpawnInfo { get; private set; }

    void OnSpawnInfoReceived(object spawnInfo)
    {
        SpawnInfo = (List<SpawnInfo>)spawnInfo;

        HasSpawnInfo = true;
        _updateCallback();
    }

    public void RequestSpawnInfo(Biome biome, HeightMap heightMap, MeshData meshData, MeshSettings meshSettings, Vector2 sampleCenter)
    {
        HasRequestedSpawnInfo = true;
        ThreadedDataRequester.RequestData(() => PrefabSpawner.SpawnOnChunk(biome, heightMap, meshData, meshSettings, sampleCenter), OnSpawnInfoReceived);
    }
}
