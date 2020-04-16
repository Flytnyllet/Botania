using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    Dictionary<Vector2, Texture2D> _renderedMapChunks;
    List<Vector2> _chunksInMap;
    List<GameObject> _spawnedMapChunks;

    [Header("Drop")]

    [SerializeField] Canvas _canvas;
    [SerializeField] MapSettings _mapSettings;
    [SerializeField] MeshSettings _meshSettings;


    [Header("General Settings")]

    [SerializeField, Range(1, 100)] float _chunkSize = 20;

    Timer _testTimer = new Timer(1);

    private void Awake()
    {
        _renderedMapChunks = new Dictionary<Vector2, Texture2D>();
        _chunksInMap = new List<Vector2>();
        _spawnedMapChunks = new List<GameObject>();   
    }

    private void Update()
    {
        _testTimer.Time += Time.deltaTime;

        if (_testTimer.Expired())
        {
            Clear();
            DisplayMap();
            _testTimer.Reset();
        }
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
        Debug.Log(chunkCoord);

        _renderedMapChunks.Add(chunkCoord, texture);
    }

    public void DisplayMap()
    {
        for (int i = 0; i < _chunksInMap.Count; i++)
        {
            if (_renderedMapChunks.ContainsKey(_chunksInMap[i]))
            {
                Texture2D texture = _renderedMapChunks[_chunksInMap[i]];

                GameObject mapChunk = new GameObject(_chunksInMap[i].ToString());
                mapChunk.transform.parent = _canvas.transform;
                mapChunk.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90f);

                //Position
                mapChunk.transform.localPosition = _chunksInMap[i] * _chunkSize * mapChunk.transform.localScale.x;

                //Add Texture
                RawImage image = mapChunk.AddComponent<RawImage>();
                image.texture = texture;
                image.rectTransform.sizeDelta = new Vector2(_chunkSize, _chunkSize);

                _spawnedMapChunks.Add(mapChunk);
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < _spawnedMapChunks.Count; i++)
        {
            Destroy(_spawnedMapChunks[i]);
        }
        _spawnedMapChunks.Clear();
    }
}
