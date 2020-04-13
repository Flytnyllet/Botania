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
        BIOME
    }

    [Header("General Settings")]

    [SerializeField, Tooltip("Should the terrainmap update in the editor?")] bool _autoUpdate;
    [SerializeField] DrawMode _drawMode;

    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] HeightMapSettings _heightMapSettings;
    [SerializeField] TextureData _textureData;
    [SerializeField] Material _terrainMaterial;

    [Header("Noise settings")]

    [SerializeField] NoiseMergeType _noiseMergeType;
    [SerializeField] NoiseSettingsData _noiseSettingsData_1;
    [SerializeField] NoiseSettingsData _noiseSettingsData_2;
    [SerializeField, Range(1, 200), Tooltip("How big should the displayed noise be?")] int _noiseViewSize = 1;

    [Header("Mesh Settings")]

    [SerializeField, Range(0, MeshSettings.NUMBER_SUPPORTED_LODS - 1)] int _editorPreviewLevelOfDetail;

    [Header("Drop")]

    [SerializeField] Renderer _textureRenderer;
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] MeshRenderer _meshRenderer;

    [Header("Biome Testing")]


    [SerializeField] Biome _biome;

    Transform _biomeContainer;


    public bool DoAutoUpdate() { return _autoUpdate; }

    public float GetScale() { return _meshSettings.MeshScale; }

    public void DrawMapInEditor()
    {
        //Used to store prefab objects in edit mode and delete them when changes are made (is a bit buggy)
        if (_biomeContainer != null)
            DestroyImmediate(_biomeContainer.gameObject);
        _biomeContainer = new GameObject("DELETE ME IF MANY OF ME").transform;

        //Apply material to mesh
        _textureData.ApplyToMaterial(_terrainMaterial);
        _textureData.UpdateMeshHeights(_terrainMaterial, _heightMapSettings.MinHeight, _heightMapSettings.MaxHeight);

        //Generate the heightmap for the chunk at origin
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine, _heightMapSettings, Vector2.zero);

        float[,] noise = Noise.MergeNoise(_meshSettings.NumVertsPerLine * _noiseViewSize, _meshSettings.NumVertsPerLine * _noiseViewSize, _noiseSettingsData_1.NoiseSettingsDataMerge, _noiseSettingsData_2.NoiseSettingsDataMerge, _noiseMergeType, Vector2.zero);

        if (_drawMode == DrawMode.NOISE_MAP)
            DrawTexture(TextureGenerator.TextureFromNoise(noise));
        else if (_drawMode == DrawMode.MESH)
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.heightMap, _meshSettings, _editorPreviewLevelOfDetail));
        else if (_drawMode == DrawMode.FALL_OF_MAP)
        {
            float[,] fallOf = (new HeightMap(FallofGenerator.GenerateFallofMap(_meshSettings.NumVertsPerLine), 1, 1).heightMap);
            DrawTexture(TextureGenerator.TextureFromNoise(fallOf));
        }
        else if (_drawMode == DrawMode.BIOME)
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.heightMap, _meshSettings, _editorPreviewLevelOfDetail);
            DrawMesh(meshData);
            PrefabSpawner prefabSpawner = new PrefabSpawner();
            List<SpawnInfo> spawnInfo = prefabSpawner.SpawnOnChunk(0, _biome, heightMap, meshData, _meshSettings, Vector2.zero, Vector2.zero);
            prefabSpawner.SpawnSpawnInfo(spawnInfo, _biomeContainer);
        }
    }

    //Draws on the plane (for noise display)
    public void DrawTexture(Texture2D texture)
    {
        _textureRenderer.sharedMaterial.mainTexture = texture;
        _textureRenderer.transform.localScale = new Vector3(texture.width / 10f, 1, texture.height / 10f);

        _textureRenderer.gameObject.SetActive(true);
        _meshFilter.gameObject.SetActive(false);
    }

    //Create the mesh from meshdata if that is to be displayed
    public void DrawMesh(MeshData meshData)
    {
        _meshFilter.sharedMesh = meshData.CreateMesh();

        _textureRenderer.gameObject.SetActive(false);
        _meshFilter.gameObject.SetActive(true);
    }

    //If changes are made, apply in edit mode
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
            _heightMapSettings.NoiseSettingsData.OnValuesUpdated -= OnValuesUpdated;
            _heightMapSettings.NoiseSettingsData.OnValuesUpdated += OnValuesUpdated;
        }
        if (_noiseSettingsData_1 != null)
        {
            _noiseSettingsData_1.OnValuesUpdated -= OnValuesUpdated;
            _noiseSettingsData_1.OnValuesUpdated += OnValuesUpdated;
        }
        if (_noiseSettingsData_2 != null)
        {
            _noiseSettingsData_2.OnValuesUpdated -= OnValuesUpdated;
            _noiseSettingsData_2.OnValuesUpdated += OnValuesUpdated;
        }
        if (_biome != null)
        {
            _biome.OnValuesUpdated -= OnValuesUpdated;
            _biome.OnValuesUpdated += OnValuesUpdated;
            SubscribeToSpawnables(_biome.HighLODSpawnable);
            SubscribeToSpawnables(_biome.MediumLODSpawnable);
            SubscribeToSpawnables(_biome.LowLODSpawnable);
        }
        if (_textureData != null)
        {
            _textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            _textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
        if (_textureData != null)
        {
            _textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            _textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }

    private void SubscribeToSpawnables(Spawnable[] spawnables)
    {
        for (int i = 0; i < spawnables.Length; i++)
        {
            spawnables[i].OnValuesUpdated -= OnValuesUpdated;
            spawnables[i].OnValuesUpdated += OnValuesUpdated;

            SubscribeToSpawnables(spawnables[i].SubSpawners);
        }
    }
}
