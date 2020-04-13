using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    //Faster comparing square distance
    static readonly float VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = 25f;
    static readonly float SQR_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE = VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE * VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE;

    [Header("Settings")]
    [SerializeField] LODInfo[] _detailLevels;
    [SerializeField, Range(0, 4), Tooltip("What LOD should the collider have?")] int _colliderLODIndex;
    [SerializeField] Biome _biome;

    [Header("Drop")]

    [SerializeField] Transform _viewer;
    [SerializeField] Material _mapMaterial;
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] HeightMapSettings _heightMapSettings;
    [SerializeField] TextureData _textureSettings;

    Vector2 _viewerPosition;
    Vector2 _viewerPositionOld;
    float _meshWorldSize;
    int _chunksVisableInViewDist;

    //Holds created chunks which are enabled or disabled
    Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> _visibleTerrainChunks = new List<TerrainChunk>();

    private void Start()
    {
        _textureSettings.ApplyToMaterial(_mapMaterial);
        _textureSettings.UpdateMeshHeights(_mapMaterial, _heightMapSettings.MinHeight, _heightMapSettings.MaxHeight);

        float maxViewDistance = _detailLevels[_detailLevels.Length - 1].visableDstThreshold;

        _meshWorldSize = _meshSettings.MeshWorldSize;
        _chunksVisableInViewDist = Mathf.RoundToInt(maxViewDistance / _meshWorldSize);

        UpdateVisableChunks();
    }

    private void Update()
    {
        _viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);

        if (_viewerPosition != _viewerPositionOld)
        {
            foreach (TerrainChunk chunk in _visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((_viewerPositionOld - _viewerPosition).sqrMagnitude > SQR_VIEWER_MOVE_THRESHOLD_FOR_CHUNK_UPDATE)
        {
            _viewerPositionOld = _viewerPosition;
            UpdateVisableChunks();
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
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, _heightMapSettings, _meshSettings, _detailLevels, _colliderLODIndex, transform, _viewer, _mapMaterial, _biome);
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
public struct LODInfo
{
    [Range(0, MeshSettings.NUMBER_SUPPORTED_LODS - 1)] public int levelOfDetail;
    [Range(0, 1000)] public float visableDstThreshold;

    public float sqrVisibleDistanceThreshold
    {
        get { return visableDstThreshold * visableDstThreshold; }
    }
}
