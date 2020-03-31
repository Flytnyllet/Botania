using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour
{
    //Faster comparing square distance
    const float VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = 25f;
    const float SQR_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE * VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE;

    public static float MaxViewDistance { get; private set; }

    [Header("Settings")]
    [SerializeField] LODInfo[] _detailLevels;

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
    List<TerrainChunk> _terrainChunksVisableLastUpdate = new List<TerrainChunk>();

    private void Start()
    {
        //Ändra eventuellt hur den hittar senare!
        _mapGenerator = FindObjectOfType<MapGenerator>();

        MaxViewDistance = _detailLevels[_detailLevels.Length - 1].visableDstThreshold;

        _chunkSize = MapGenerator.MAP_CHUNK_SIZE - 1;
        _chunksVisableInViewDist = Mathf.RoundToInt(MaxViewDistance / _chunkSize);

        UpdateVisableChunks();
    }

    private void Update()
    {
        ViewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);

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
                {
                    _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (_terrainChunkDictionary[viewedChunkCoord].IsVisable())
                    {
                        _terrainChunksVisableLastUpdate.Add(_terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    _terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, _chunkSize, _detailLevels, transform, _mapMaterial));
                }
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

        LODInfo[] _detailLevels;
        LODMesh[] _lodMeshes;

        MapData _mapData;
        bool _mapDataReceived;
        int _previousLODIndex = -1;
        
        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this._detailLevels = detailLevels;

            _position = coord * size;
            _bounds = new Bounds(_position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);

            _meshObject = new GameObject("Terrain Chunk");
            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshRenderer.material = material;

            _meshObject.transform.position = positionV3;
            _meshObject.transform.parent = parent;
            SetVisable(false);

            _lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < _detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(_detailLevels[i].levelOfDetail, UpdateTerrainChunk);
            }

            _mapGenerator.RequestMapData(_position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
            this._mapData = mapData;
            _mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.MAP_CHUNK_SIZE, MapGenerator.MAP_CHUNK_SIZE);
            _meshRenderer.material.mainTexture = texture;

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
                }

                SetVisable(visable);
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

        System.Action _updateCallback;

        public LODMesh(int levelOfDetail, System.Action updateCallback)
        {
            this._levelOfDetail = levelOfDetail;
            this._updateCallback = updateCallback;
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
        [Range(0, 6)]    public int levelOfDetail;
        [Range(0, 1000)] public float visableDstThreshold;
    }

}
