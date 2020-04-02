using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour
{
    //Faster comparing square distance
    static readonly float VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = 25f;
    static readonly float SQR_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE * VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE;
    static readonly float COLLIDER_GENERATION_DISTANCE_THRESHOLD = 5;

    public static float MaxViewDistance { get; private set; }

    [Header("Settings")]
    [SerializeField] LODInfo[] _detailLevels;
    [SerializeField] int _colliderLODIndex;

    [Header("Drop")]

    [SerializeField] Transform _viewer;
    [SerializeField] Material _mapMaterial;

    public static Vector2 ViewerPosition { get; private set; }
    Vector2 _viewerPositionOld;
    static MapGenerator _mapGenerator;
    int _chunkSize;
    int _chunksVisableInViewDist;

    //Holds created chunks which are enabled or disabled
    Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> _terrainChunksVisableLastUpdate = new List<TerrainChunk>();

    private void Start()
    {
        //Ändra eventuellt hur den hittar senare!
        _mapGenerator = FindObjectOfType<MapGenerator>();

        MaxViewDistance = _detailLevels[_detailLevels.Length - 1].visableDstThreshold;

        _chunkSize = MapGenerator.GetChunkSize() - 1;
        _chunksVisableInViewDist = Mathf.RoundToInt(MaxViewDistance / _chunkSize);

        UpdateVisableChunks();
    }

    private void Update()
    {
        ViewerPosition = new Vector2(_viewer.position.x, _viewer.position.z) / _mapGenerator.GetScale();

        if (ViewerPosition != _viewerPositionOld)
        {
            foreach (TerrainChunk chunk in _terrainChunksVisableLastUpdate)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((_viewerPositionOld - ViewerPosition).sqrMagnitude > SQR_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE)
        {
            _viewerPositionOld = ViewerPosition;
            UpdateVisableChunks();
        }
    }

    private void UpdateVisableChunks()
    {
        for (int i = 0; i < _terrainChunksVisableLastUpdate.Count; i++)
        {
            _terrainChunksVisableLastUpdate[i].SetVisable(false);
        }
        _terrainChunksVisableLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / _chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / _chunkSize);

        for (int yOffset = -_chunksVisableInViewDist; yOffset <= _chunksVisableInViewDist; yOffset++)
        {
            for (int xOffset = -_chunksVisableInViewDist; xOffset <= _chunksVisableInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                else
                    _terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, _chunkSize, _detailLevels, _colliderLODIndex, transform, _mapMaterial));
            }
        }
    }

    public class TerrainChunk
    {
        GameObject _meshObject;
        Vector2 _position;
        Bounds _bounds;

        MeshRenderer _meshRenderer;
        MeshFilter _meshFilter;
        MeshCollider _meshCollider;

        LODInfo[] _detailLevels;
        LODMesh[] _lodMeshes;
        int _colliderLODIndex;

        MapData _mapData;
        bool _mapDataReceived;
        int _previousLODIndex = -1;
        bool _hasSetCollider = false;
        
        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Material material)
        {
            this._detailLevels = detailLevels;
            this._colliderLODIndex = colliderLODIndex;

            _position = coord * size;
            _bounds = new Bounds(_position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);

            _meshObject = new GameObject("Terrain Chunk");
            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshCollider = _meshObject.AddComponent<MeshCollider>();
            _meshRenderer.material = material;

            _meshObject.transform.position = positionV3 * _mapGenerator.GetScale();
            _meshObject.transform.parent = parent;
            _meshObject.transform.localScale = Vector3.one * _mapGenerator.GetScale();
            SetVisable(false);

            _lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < _detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(_detailLevels[i].levelOfDetail);
                _lodMeshes[i]._updateCallback += UpdateTerrainChunk;
                if (i == colliderLODIndex)
                    _lodMeshes[i]._updateCallback += UpdateCollisionMesh;
            }

            _mapGenerator.RequestMapData(_position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
            this._mapData = mapData;
            _mapDataReceived = true;

            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (_mapDataReceived)
            {
                float viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));
                bool visable = viewerDstFromNearestEdge <= MaxViewDistance;

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
                            lodMesh.RequestMesh(_mapData);
                    }

                    _terrainChunksVisableLastUpdate.Add(this);
                }

                SetVisable(visable);
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
                        _lodMeshes[_colliderLODIndex].RequestMesh(_mapData);
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
        public bool HasRequestedMesh { get; private set; }
        public bool HasMesh { get; private set; }
        int _levelOfDetail;

        public event System.Action _updateCallback;

        public LODMesh(int levelOfDetail)
        {
            this._levelOfDetail = levelOfDetail;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            Mesh = meshData.CreateMesh();
            HasMesh = true;

            _updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            HasRequestedMesh = true;
            _mapGenerator.RequestMeshData(mapData, _levelOfDetail, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        [Range(0, MeshGenerator.NUMBER_SUPPORTED_LODS - 1)]    public int levelOfDetail;
        [Range(0, 1000)] public float visableDstThreshold;
        [Tooltip("Should this level of detail have a collider? (no if not on player)")]public bool useForCollider;

        public float sqrVisibleDistanceThreshold
        {
            get { return visableDstThreshold * visableDstThreshold; }
        }
    }

}
