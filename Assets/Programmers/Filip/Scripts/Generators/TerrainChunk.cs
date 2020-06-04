using UnityEngine;
using System.Collections.Generic;

public class TerrainChunk
{
    static readonly float COLLIDER_GENERATION_DISTANCE_THRESHOLD = 35;

    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 Coord { get; private set; }

    GameObject _meshObject;
    Vector2 _sampleCenter;
    public Vector2 SampleCenter { get => _sampleCenter; }
    Vector2 _chunkCoord;
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
    public MeshSettings MeshSettings { get => _meshSettings; }
    Transform _viewer;

    SpawnInfoRequester _spawnInfoRequester;
    Biome _biome;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material, Biome biome, GroundMaterialGenerator materialGenerator, string groundLayer)
    {
        this._biome = biome;
        this.Coord = coord;
        this._detailLevels = detailLevels;
        this._colliderLODIndex = colliderLODIndex;
        this._heightMapSettings = heightMapSettings;
        this._meshSettings = meshSettings;
        this._viewer = viewer;
        this._chunkCoord = coord;


        _sampleCenter = coord * meshSettings.MeshWorldSize / meshSettings.MeshScale;
        Vector2 position = coord * meshSettings.MeshWorldSize;
        _bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);

        _meshObject = new GameObject("Terrain Chunk: " + coord);
        _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshFilter = _meshObject.AddComponent<MeshFilter>();
        _meshCollider = _meshObject.AddComponent<MeshCollider>();
        _meshRenderer.material = materialGenerator.MakeMaterial(MeshSettings.ChunkSize, _sampleCenter, _meshRenderer);

        _spawnInfoRequester = new SpawnInfoRequester(_biome, _meshFilter.gameObject);

        _meshObject.transform.position = new Vector3(position.x, 0, position.y);
        _meshObject.transform.parent = parent;
        _meshObject.SetActive(false);

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
        _meshObject.layer = LayerMask.NameToLayer(groundLayer);
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine, _heightMapSettings, _sampleCenter), OnHeightMapReceived);
    }

    public void UpdateRenderDistance(LODInfo[] lodInfo)
    {
        _detailLevels = lodInfo;
        _maxViewDistance = _detailLevels[_detailLevels.Length - 1].visableDstThreshold;
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

                if (_lodMeshes[lodIndex].HasMesh) //The mesh is spawned but no prefabs on it (it's also the highest detail level)
                {
                    _spawnInfoRequester.Refresh(lodIndex); //Used to disable different spawnContainers due to LOD

                    if (!_spawnInfoRequester.IsSet(lodIndex))
                    {
                        if (_spawnInfoRequester.HasSpawnInfo(lodIndex) && wasVisible) //The requested spawninfo has arrived! Hurray!
                        {
                            _spawnInfoRequester.Set(lodIndex);
                            _spawnInfoRequester.SpawnSpawnInfo(_lodMeshes[lodIndex].MeshData, _meshSettings.ChunkSize, lodIndex, _meshFilter.transform);
                        }
                        else if (!_spawnInfoRequester.HasRequestedSpawnInfo) //Request spawninfo!
                            _spawnInfoRequester.RequestSpawnInfo(lodIndex, _detailLevels[lodIndex].levelOfDetail, _heightMap, _lodMeshes[lodIndex].MeshData, _meshSettings, new Vector2(_sampleCenter.x, -_sampleCenter.y), _chunkCoord, _meshFilter.transform);
                    }
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
        if (visable || _spawnInfoRequester.CompletelySpawnedIn)
            _meshObject.SetActive(visable);
        else if (!_spawnInfoRequester.CompletelySpawnedIn)
            _spawnInfoRequester.Disable(_meshObject);
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
    Biome _thisBiome;

    //This script has to be a monobehaviour as to be able to use coroutines 
    PrefabSpawner _prefabSpawner;

    bool[] _isSet = new bool[3];
    bool[] _hasSpawnInfo = new bool[3];
    int _currentSpawningLodIndex = -1;

    Transform[] _spawnContainers = new Transform[3];
    List<SpawnInfo>[] _spawnInfo = new List<SpawnInfo>[3];


    public bool HasRequestedSpawnInfo { get; private set; }

    public event System.Action _updateCallback;

    public bool CompletelySpawnedIn { get { return _prefabSpawner.CompletelySpawnedIn; } }
    public bool IsSet(int index) { return _isSet[index]; }
    public bool HasSpawnInfo(int index) { return _hasSpawnInfo[index]; }

    bool _areNormalSet = false;

    public SpawnInfoRequester(Biome biome, GameObject container)
    {
        this._thisBiome = new Biome(biome);
        this._prefabSpawner = container.AddComponent<PrefabSpawner>();
    }

    public void Set(int index)
    {
        _isSet[index] = true;
    }

    public void Disable(GameObject parent)
    {
        _prefabSpawner.Disable(parent);
    }

    public List<SpawnInfo> GetSpawnInfo(int index)
    {
        return _spawnInfo[index];
    }

    public void Refresh(int index)
    {
        for (int i = 0; i < _spawnContainers.Length; i++)
        {
            if (_spawnContainers[i] != null)
            {
                if (i < index)
                    _spawnContainers[i].gameObject.SetActive(false);
                else if (i >= index)
                    _spawnContainers[i].gameObject.SetActive(true);
            }
        }
    }

    void OnSpawnInfoReceived(object spawnInfo)
    {
        if (_spawnInfo[_currentSpawningLodIndex] == null)
        {
            _spawnInfo[_currentSpawningLodIndex] = (List<SpawnInfo>)spawnInfo;

            _hasSpawnInfo[_currentSpawningLodIndex] = true;
            HasRequestedSpawnInfo = false; //Sets to false to indicate that if the lod increase it can spawn more!
            _updateCallback();
        }
    }

    public void RequestSpawnInfo(int detailType, int levelOfDetail, HeightMap heightMap, MeshData meshData, MeshSettings meshSettings, Vector2 sampleCenter, Vector2 chunkCoord, Transform container)
    {
        OnFirstSpawn(detailType, levelOfDetail, heightMap, meshData, meshSettings, sampleCenter, chunkCoord, container);

        HasRequestedSpawnInfo = true;
        _currentSpawningLodIndex = detailType;
        ThreadedDataRequester.RequestData(() => _prefabSpawner.SpawnOnChunk(detailType, levelOfDetail, _thisBiome, heightMap, meshData, meshSettings, sampleCenter, chunkCoord), OnSpawnInfoReceived);
    }

    public void SpawnSpawnInfo(MeshData meshData, int chunkSize, int lodIndex, Transform container)
    {
        GameObject spawnInfoContainer = new GameObject("SpawnContainer " + lodIndex + " Count: " + _spawnInfo[lodIndex].Count);

        spawnInfoContainer.transform.parent = container;
        _spawnContainers[lodIndex] = spawnInfoContainer.transform;
        _prefabSpawner.SpawnSpawnInfo(_spawnInfo[lodIndex], spawnInfoContainer.transform, lodIndex == 0);

        //Sets normals for everything in LOD 1 and 2
        if (lodIndex == 0 && !_areNormalSet)
        {
            _areNormalSet = true;
            _prefabSpawner.SetNormals(meshData, chunkSize);
        }
    }

    //Called only when loading in high LOD first, (spawn or teleport -> NOT THREADED!)
    void OnFirstSpawn(int detailType, int levelOfDetail, HeightMap heightMap, MeshData meshData, MeshSettings meshSettings, Vector2 sampleCenter, Vector2 chunkCoord, Transform container)
    {
        if (detailType == 2 || IsSet(1) || (IsSet(0)))
            return;
        //The chunk to spawn is in LOD 1 range but LOD 2 hasn't spawned yet, spawn it!
        else if (!IsSet(2) && detailType == 1)
        {
            SpawnFromMainThread(2, levelOfDetail, heightMap, meshData, meshSettings, sampleCenter, chunkCoord, container);
        }
        //The chunk to spawn is in LOD 0 range but LOD 1 and 2 hasn't spawned yet, spawn them!
        else if (!IsSet(2) && !IsSet(1) && detailType == 0)
        {
            SpawnFromMainThread(2, levelOfDetail, heightMap, meshData, meshSettings, sampleCenter, chunkCoord, container);
            SpawnFromMainThread(1, levelOfDetail, heightMap, meshData, meshSettings, sampleCenter, chunkCoord, container);
        }
        //The chunk to spawn is in LOD 0 range but LOD 1 hasn't spawned yet, spawn it!
        else if (!IsSet(1) && IsSet(2) && detailType == 0)
        {
            SpawnFromMainThread(1, levelOfDetail, heightMap, meshData, meshSettings, sampleCenter, chunkCoord, container);
        }
    }

    //Not threaded as it should only be called on SPAWN
    void SpawnFromMainThread(int detailType, int levelOfDetail, HeightMap heightMap, MeshData meshData, MeshSettings meshSettings, Vector2 sampleCenter, Vector2 chunkCoord, Transform container)
    {
        Set(detailType);
        _hasSpawnInfo[detailType] = true;
        if (_spawnInfo[detailType] == null)
            _spawnInfo[detailType] = _prefabSpawner.SpawnOnChunk(detailType, levelOfDetail, _thisBiome, heightMap, meshData, meshSettings, sampleCenter, chunkCoord);
        
        SpawnSpawnInfo(meshData, meshSettings.ChunkSize, detailType, container);
    }
}
