using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
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

    public bool DoAutoUpdate() { return _autoUpdate; }

    public float GetScale() { return _meshSettings.MeshScale; }

    public void DrawMapInEditor()
    {
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
        if (_textureData != null)
        {
            _textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            _textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }
}
