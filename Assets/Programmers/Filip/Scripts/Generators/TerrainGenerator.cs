using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    //Faster comparing square distance
    static readonly float VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = 20f;
    static readonly float SQR_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE * VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE;

    public static int RenderDistanceIndex { get; private set; } = 0;

    public static void SetRenderDistanceOnStart(int index)
    {
        RenderDistanceIndex = index;
    }

    [Header("Settings")]
    [SerializeField] LODInfoHolder[] _detailLevelsHolder;
    [SerializeField, Range(0, 10)] int _detailLevelIndex = 3;
    [SerializeField, Range(0, 4), Tooltip("What LOD should the collider have?")] int _colliderLODIndex;
    [SerializeField] Biome _biome;
    [SerializeField] string _groundLayer;

    [Header("Drop")]

    [SerializeField] Material _mapMaterial;
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] HeightMapSettings _heightMapSettings;
    //[SerializeField] TextureData _textureSettings;
    [SerializeField] GroundMaterialGenerator _textureSettings;

    Transform _viewer;
    Vector2 _viewerPosition;
    Vector2 _viewerPositionOld;
    float _meshWorldSize;
    int _chunksVisableInViewDist;

    //Holds created chunks which are enabled or disabled
    Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> _visibleTerrainChunks = new List<TerrainChunk>();

    Timer _spawnTimer;

    private void Awake()
    {
        RenderDistanceIndex = _detailLevelIndex;

        _spawnTimer = new Timer(1f);
    }

    private void OnValidate()
    {
        if (_detailLevelIndex >= _detailLevelsHolder.Length)
            _detailLevelIndex = _detailLevelsHolder.Length - 1;
    }

    private void Start()
    {
        _viewer = Player.GetPlayerTransform();

        _viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);
        _viewerPositionOld = _viewerPosition;

        StartCoroutine(OnStart());
    }

    IEnumerator OnStart()
    {
        while (!SaveSystem.Ready)
            yield return null;

        float maxViewDistance = _detailLevelsHolder[_detailLevelIndex]._levelOfDetail[_detailLevelsHolder[_detailLevelIndex]._levelOfDetail.Length - 1].visableDstThreshold;

        _meshWorldSize = _meshSettings.MeshWorldSize;
        _chunksVisableInViewDist = Mathf.RoundToInt(maxViewDistance / _meshWorldSize);

        UpdateVisableChunks();
    }

    public void ChangeRenderDistance(int index)
    {
        _detailLevelIndex = index;
        RenderDistanceIndex = _detailLevelIndex;

        float maxViewDistance = _detailLevelsHolder[_detailLevelIndex]._levelOfDetail[_detailLevelsHolder[_detailLevelIndex]._levelOfDetail.Length - 1].visableDstThreshold;
        _chunksVisableInViewDist = Mathf.RoundToInt(maxViewDistance / _meshWorldSize);

        foreach(KeyValuePair<Vector2, TerrainChunk> entry in _terrainChunkDictionary)
        {
            entry.Value.UpdateRenderDistance(_detailLevelsHolder[_detailLevelIndex]._levelOfDetail);
        }
        UpdateVisableChunks();
    }

    private void Update()
    {
        if (SaveSystem.Ready)
        {
            _spawnTimer.Time += Time.deltaTime;
            _viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);

            if (_viewerPosition != _viewerPositionOld || !_spawnTimer.Expired())
            {
                for (int i = 0; i < _visibleTerrainChunks.Count; i++)
                {
                    _visibleTerrainChunks[i].UpdateCollisionMesh();
                }
            }

            if ((_viewerPositionOld - _viewerPosition).sqrMagnitude > SQR_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE)
            {
                _viewerPositionOld = _viewerPosition;
                UpdateVisableChunks();
            }
        }
    }

    private void UpdateVisableChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = _visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(_visibleTerrainChunks[i].Coord);
            _visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _meshWorldSize);

        for (int yOffset = -_chunksVisableInViewDist; yOffset <= _chunksVisableInViewDist; yOffset++)
        {
            for (int xOffset = -_chunksVisableInViewDist; xOffset <= _chunksVisableInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                        _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, _heightMapSettings, _meshSettings, _detailLevelsHolder[_detailLevelIndex]._levelOfDetail, _colliderLODIndex, transform, _viewer, _mapMaterial, _biome, _textureSettings, _groundLayer);

                        //Make mapchunk
                        MapGenerator.AddChunkToMap(viewedChunkCoord);

                        _terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();
                    }
                }
            }
        }
    }

    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
            _visibleTerrainChunks.Add(chunk);
        else
            _visibleTerrainChunks.Remove(chunk);
    }
}

[System.Serializable]
public class LODInfoHolder
{
    public LODInfo[] _levelOfDetail;
}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.NUMBER_SUPPORTED_LODS - 1)] public int levelOfDetail;
    [Range(0, 1000)] public float visableDstThreshold;

    public float sqrVisibleDistanceThreshold
    {
        get { return visableDstThreshold * visableDstThreshold; }
    }
}
