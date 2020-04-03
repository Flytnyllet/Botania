using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public enum DrawMode
    {
        NOISE_MAP,
        MESH,
        FALL_OF_MAP,
        SPAWNABLE_NOISE,
        BIOME
    }

    [Header("General Settings")]

    [SerializeField] DrawMode _drawMode;
    [SerializeField, Tooltip("Should the terrainmap update in the editor?")] bool _autoUpdate;

    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] HeightMapSettings _heightMapSettings;
    [SerializeField] TextureData _textureData;
    [SerializeField] Material _terrainMaterial;

    [Header("Mesh Settings")]

    [SerializeField, Range(0, MeshSettings.NUMBER_SUPPORTED_LODS - 1)] int _editorPreviewLevelOfDetail;

    [Header("Drop")]

    [SerializeField] Renderer _textureRenderer;
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] MeshRenderer _meshRenderer;

    [Header("Biome Testing")]

    [SerializeField] NoiseMergeType _noiseMergeType;

    [SerializeField] Biome _biome;

    [SerializeField] Spawnable _spawnable_1;
    [SerializeField] Spawnable _spawnable_2;

    Transform _biomeContainer;

    public bool DoAutoUpdate() { return _autoUpdate; }

    public float GetScale() { return _meshSettings.MeshScale; }

    public void DrawMapInEditor()
    {
        if (_biomeContainer != null)
            DestroyImmediate(_biomeContainer.gameObject);

        _biomeContainer = new GameObject().transform;
        _biomeContainer.parent = transform;

        _textureData.ApplyToMaterial(_terrainMaterial);
        _textureData.UpdateMeshHeights(_terrainMaterial, _heightMapSettings.MinHeight, _heightMapSettings.MaxHeight);

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(_meshSettings.ChunkSize + 2, _meshSettings.ChunkSize + 2, _heightMapSettings, Vector2.zero);
        //Ändra senare kan vara slow

        if (_drawMode == DrawMode.NOISE_MAP)
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        else if (_drawMode == DrawMode.MESH)
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.heightMap, _meshSettings, _editorPreviewLevelOfDetail));
        else if (_drawMode == DrawMode.FALL_OF_MAP)
            DrawTexture(TextureGenerator.TextureFromHeightMap( new HeightMap(FallofGenerator.GenerateFallofMap(_meshSettings.ChunkSize), 0, 1)));
        else if (_drawMode == DrawMode.SPAWNABLE_NOISE)
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(Noise.MergeNoise(_meshSettings.ChunkSize + 2, _meshSettings.ChunkSize + 2, _spawnable_1.NoiseSettings, _spawnable_2.NoiseSettings, _noiseMergeType, Vector2.zero), 0, 1)));
        else if (_drawMode == DrawMode.BIOME)
        {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.heightMap, _meshSettings, _editorPreviewLevelOfDetail));
            PrefabSpawner.SpawnOnChunk(_biome, heightMap, _meshSettings, _biomeContainer, Vector2.zero);
        }
    }


    public void DrawTexture(Texture2D texture)
    {
        _textureRenderer.sharedMaterial.mainTexture = texture;
        _textureRenderer.transform.localScale = new Vector3(texture.width / 10f, 1, texture.height / 10f);

        _textureRenderer.gameObject.SetActive(true);
        _meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData)
    {
        _meshFilter.sharedMesh = meshData.CreateMesh();

        _textureRenderer.gameObject.SetActive(false);
        _meshFilter.gameObject.SetActive(true);
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
        if (_spawnable_1 != null)
        {
            _spawnable_1.OnValuesUpdated -= OnValuesUpdated;
            _spawnable_1.OnValuesUpdated += OnValuesUpdated;
        }
        if (_spawnable_2 != null)
        {
            _spawnable_2.OnValuesUpdated -= OnValuesUpdated;
            _spawnable_2.OnValuesUpdated += OnValuesUpdated;
        }
        if (_biome != null)
        {
            _biome.OnValuesUpdated -= OnValuesUpdated;
            _biome.OnValuesUpdated += OnValuesUpdated;
        }
        if (_textureData != null)
        {
            _textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            _textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }
}
