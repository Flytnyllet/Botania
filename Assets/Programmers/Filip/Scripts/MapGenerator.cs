using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    Dictionary<Vector2, Texture2D> _renderedMapChunks;
    List<Vector2> _chunksInMap;
    List<GameObject> _spawnedMapChunks;
    bool _displaying = false;

    [Header("Drop")]

    [SerializeField] Map _mapScript;
    [SerializeField] MapSettings _mapSettings;
    [SerializeField] MeshSettings _meshSettings;


    [Header("General Settings")]

    [SerializeField, Range(1, 100)] float _chunkSize = 20;

    private void Awake()
    {
        _renderedMapChunks = new Dictionary<Vector2, Texture2D>();
        _chunksInMap = new List<Vector2>();
        _spawnedMapChunks = new List<GameObject>();

        for (int x = -20; x < 20; x++)
        {
            for (int y = -20; y < 20; y++)
            {
                AddChunkToMap(new Vector2(x, y));
            }
        }

        Display(true);
    }

    public void AddChunkToMap(Vector2 chunkCoord)
    {
        _chunksInMap.Add(chunkCoord);
        Vector2 sampleCenter = chunkCoord * _meshSettings.MeshWorldSize / _meshSettings.MeshScale;

        ThreadedDataRequester.RequestData(() => RequestTextureChunkData(chunkCoord, sampleCenter), ReceivedTextureChunkData);
    }

    private TextureChunkData RequestTextureChunkData(Vector2 chunkCoord, Vector2 sampleCenter)
    {
        return TextureGenerator.DrawMap(_meshSettings.NumVertsPerLine, _mapSettings, sampleCenter, chunkCoord, 1);
    }

    private void ReceivedTextureChunkData(object data)
    {
        TextureChunkData thisData = (TextureChunkData)data;

        AddTexture(TextureGenerator.TextureFromColorMap(thisData.colorMap, thisData.width, thisData.height), thisData.chunkCoord);
    }

    private void AddTexture(Texture2D texture, Vector2 chunkCoord)
    {
        if (!_renderedMapChunks.ContainsKey(chunkCoord))
            _renderedMapChunks.Add(chunkCoord, texture);
        else
            Debug.LogError("This terrainchunk is already a key for a texture? Multiple callings??");

        InstantiateChunk(texture, chunkCoord);
    }

    private void InstantiateChunk(Texture2D texture, Vector2 chunkCoord)
    {
        //Create GameObject
        GameObject mapChunk = new GameObject(chunkCoord.ToString());
        mapChunk.transform.parent = _mapScript.SpawnContainer;
        mapChunk.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90f);

        //Position
        mapChunk.transform.localScale = Vector3.one;
        mapChunk.transform.localPosition = chunkCoord * _chunkSize;

        //Add Texture
        RawImage image = mapChunk.AddComponent<RawImage>();
        image.texture = texture;
        image.rectTransform.sizeDelta = new Vector2(_chunkSize, _chunkSize);
        image.raycastTarget = false;

        //Display mode
        mapChunk.SetActive(_displaying);

        _spawnedMapChunks.Add(mapChunk);
    }

    private void Display(bool status)
    {
        if (_displaying != status)
        {
            _displaying = status;

            for (int i = 0; i < _spawnedMapChunks.Count; i++)
            {
                _spawnedMapChunks[i].SetActive(status);
            }
        }
    }
}
