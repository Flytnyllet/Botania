using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour, IDragHandler, IScrollHandler
{

    [Header("Drop")]

    [SerializeField] MapSettings _mapSettings;
    [SerializeField] MeshSettings _meshSettings;

    [Header("Setup")]

    [SerializeField] Canvas _parentCanvas;
    [SerializeField] RectTransform _mapHolder;
    [SerializeField] RectTransform _spawnContainer;
    [SerializeField] RectTransform _pivotSpawnContainer;
    [SerializeField] Image _playerIcon;

    [Header("General Settings")]

    [SerializeField, Range(1, 100)] float _chunkSize = 20;

    [SerializeField, Range(0.01f, 20)] float _dragSpeed = 1;
    [SerializeField, Range(0, 4)] float _zoomInPercentage = 1.33f;
    [SerializeField, Range(0, 1)] float _zoomOutPercentage = 0.75f;

    [SerializeField, Range(0, 15)] float _maxScale = 7.5f;
    [SerializeField, Range(0, 15)] float _minScale = 0.5f;

    Dictionary<Vector2, Texture2D> _renderedMapChunks;
    Transform _viewer;
    List<GameObject> _spawnedMapChunks;
    bool _displaying = false;

    private void Awake()
    {
        _spawnedMapChunks = new List<GameObject>();
        _renderedMapChunks = new Dictionary<Vector2, Texture2D>();

        ////ONLY TESTING PURPOSES!
        //for (int x = -20; x < 20; x++)
        //{
        //    for (int y = -20; y < 20; y++)
        //    {
        //        AddChunkToMap(new Vector2(x, y));
        //    }
        //}

        _playerIcon.gameObject.SetActive(false);

        Display(true);
    }

    private void Start()
    {
        _viewer = Player.GetPlayerTransform();
    }

    private void Update()
    {
        if (_displaying)
        {
            //Update player icon
            Vector3 newPosition = new Vector3(_viewer.position.x / _meshSettings.MeshWorldSize * _chunkSize, _viewer.position.z / _meshSettings.MeshWorldSize * _chunkSize, 0.0f);
            _playerIcon.rectTransform.anchoredPosition = newPosition;
            _playerIcon.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -_viewer.rotation.eulerAngles.y);
        }
    }

    public void FocusOnPlayer()
    {
        _pivotSpawnContainer.localPosition = Vector3.zero;
        _mapHolder.localPosition = -_playerIcon.rectTransform.localPosition;
    }


    //Map Manipulation
    public void OnDrag(PointerEventData eventData)
    {
        _mapHolder.anchoredPosition += eventData.delta * _dragSpeed / _pivotSpawnContainer.localScale.x / _parentCanvas.scaleFactor;
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 oldPosition = _mapHolder.position;
        _pivotSpawnContainer.position = eventData.position;

        float newScale;

        if (eventData.scrollDelta.y > 0)
            newScale = _pivotSpawnContainer.localScale.x * _zoomInPercentage;
        else
            newScale = _pivotSpawnContainer.localScale.x * _zoomOutPercentage;

        newScale = Mathf.Clamp(newScale, _minScale, _maxScale);

        _mapHolder.position = oldPosition;

        _pivotSpawnContainer.localScale = new Vector3(newScale, newScale, newScale);
    }


    public void AddChunkToMap(Vector2 chunkCoord)
    {
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
        mapChunk.transform.parent = _spawnContainer;
        mapChunk.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90f);

        //Position
        mapChunk.transform.localScale = Vector3.one;
        mapChunk.transform.localPosition = chunkCoord * _chunkSize;

        //Add Texture
        RawImage image = mapChunk.AddComponent<RawImage>();
        image.texture = texture;
        image.rectTransform.sizeDelta = new Vector2(_chunkSize, _chunkSize);
        image.raycastTarget = false;

        mapChunk.SetActive(_displaying);
        _spawnedMapChunks.Add(mapChunk);
    }

    public void Display(bool status)
    {
        if (_displaying != status)
        {
            _displaying = status;

            _playerIcon.gameObject.SetActive(status);

            for (int i = 0; i < _spawnedMapChunks.Count; i++)
            {
                _spawnedMapChunks[i].SetActive(status);
            }
        }
    }
}
