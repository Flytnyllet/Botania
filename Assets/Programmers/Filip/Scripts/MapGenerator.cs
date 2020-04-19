using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour, IDragHandler, IScrollHandler, IPointerDownHandler
{
    static MapGenerator _singletonMapGenerator;

    static MeshSettings _meshSettings;
    static MapSettings _mapSettings;
    static RectTransform _spawnContainer;
    static RectTransform _markersContainer;

    static float _chunkSize;
    static float _markerSize;

    [Header("Drop")]

    [SerializeField] MapSettings _mapSettingsInstance;
    [SerializeField] MeshSettings _meshSettingsInstance;
    [SerializeField] Sprite _waypoint;

    [Header("Setup")]

    [SerializeField] Canvas _parentCanvas;
    [SerializeField] RectTransform _mapHolder;
    [SerializeField] RectTransform _spawnContainerInstance;
    [SerializeField] RectTransform _pivotSpawnContainer;
    [SerializeField] RectTransform _markersContainerInstance;
    [SerializeField] Image _playerIcon;

    [Header("General Settings")]

    [SerializeField, Range(1, 100)] float _chunkSizeInstance = 20;
    [SerializeField, Range(1, 100)] float _markerSizeInstance = 5;
    [SerializeField, Range(1, 100)] float _waypointSize = 5;

    [SerializeField, Range(0.01f, 20)] float _dragSpeed = 1;
    [SerializeField, Range(0, 4)] float _zoomInPercentage = 1.33f;
    [SerializeField, Range(0, 1)] float _zoomOutPercentage = 0.75f;

    [SerializeField, Range(0, 15)] float _maxScale = 7.5f;
    [SerializeField, Range(0, 15)] float _minScale = 0.5f;

    static Dictionary<Vector2, TextureSave> _renderedMapChunks;
    static List<WorldMarker> _worldMarkers;
    static Transform _viewer;
    static List<GameObject> _spawnedMapChunks;
    static List<GameObject> _spawnedWorldMarkers;
    static List<GameObject> _spawnedWaypoints;
    static bool _displaying = false;

    RectTransform _canvasRectTransform;

    private void Awake()
    {
        if (_singletonMapGenerator == null)
        {
            _singletonMapGenerator = this;

            _spawnedMapChunks = new List<GameObject>();
            _spawnedWorldMarkers = new List<GameObject>();
            _spawnedWaypoints = new List<GameObject>();
            _renderedMapChunks = new Dictionary<Vector2, TextureSave>();
            _worldMarkers = new List<WorldMarker>();

            _meshSettings = _meshSettingsInstance;
            _mapSettings = _mapSettingsInstance;
            _chunkSize = _chunkSizeInstance;
            _spawnContainer = _spawnContainerInstance;
            _markersContainer = _markersContainerInstance;
            _markerSize = _markerSizeInstance;

            _canvasRectTransform = _parentCanvas.GetComponent<RectTransform>();

            _playerIcon.gameObject.SetActive(false);


            Display(true); //ONLY TESTING
        }
        else
            Destroy(_parentCanvas.gameObject);
    }

    private void Start()
    {
        _viewer = Player.GetPlayerTransform();
    }

    private void Update()
    {
        if (_displaying)
        {
            UpdatePlayerIcon();
        }
    }

    private void UpdatePlayerIcon()
    {
        Vector3 newPosition = new Vector3(_viewer.position.x / _meshSettings.MeshWorldSize * _chunkSize, _viewer.position.z / _meshSettings.MeshWorldSize * _chunkSize, 0.0f);
        _playerIcon.rectTransform.anchoredPosition = newPosition;
        _playerIcon.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -_viewer.rotation.eulerAngles.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Position
            float x = (eventData.position.x / Screen.width - 0.5f) * _canvasRectTransform.sizeDelta.x / _pivotSpawnContainer.localScale.x;
            float y = (eventData.position.y / Screen.height - 0.5f) * _canvasRectTransform.sizeDelta.y / _pivotSpawnContainer.localScale.y;
            Vector3 waypointPosition = new Vector3(x, y, 0.0f) - _mapHolder.localPosition - _pivotSpawnContainer.localPosition / _pivotSpawnContainer.localScale.x;

            GameObject newWaypoint = SpawnMarker(_waypoint, waypointPosition, _waypointSize, true);

            _spawnedWaypoints.Add(newWaypoint);
        }
    }

    #region Map Manipulation
    public void FocusOnPlayer()
    {
        _pivotSpawnContainer.localPosition = Vector3.zero;
        _mapHolder.localPosition = -_playerIcon.rectTransform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
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
    #endregion

    #region Add texture chunk to map
    public static void AddChunkToMap(Vector2 chunkCoord)
    {
        Vector2 sampleCenter = chunkCoord * _meshSettings.MeshWorldSize / _meshSettings.MeshScale;

        ThreadedDataRequester.RequestData(() => RequestTextureChunkData(chunkCoord, sampleCenter), ReceivedTextureChunkData);
    }

    private static TextureChunkData RequestTextureChunkData(Vector2 chunkCoord, Vector2 sampleCenter)
    {
        return TextureGenerator.DrawMap(_meshSettings.NumVertsPerLine, _mapSettings, sampleCenter, chunkCoord, 1);
    }

    private static void ReceivedTextureChunkData(object data)
    {
        TextureChunkData thisData = (TextureChunkData)data;

        AddTexture(TextureGenerator.TextureFromColorMap(thisData.colorMap, thisData.width, thisData.height), thisData.chunkCoord);
    }

    private static void AddTexture(Texture2D texture, Vector2 chunkCoord)
    {
        if (!_renderedMapChunks.ContainsKey(chunkCoord))
            _renderedMapChunks.Add(chunkCoord, new TextureSave(texture, chunkCoord));
        else
            Debug.LogError("This terrainchunk is already a key for a texture? Multiple callings??");

        InstantiateChunk(texture, chunkCoord);
    }

    private static void InstantiateChunk(Texture2D texture, Vector2 chunkCoord)
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
    #endregion

    public static void AddWorldMarker(Sprite sprite, Vector3 worldPosition)
    {
        Vector3 markerPosition = new Vector3(worldPosition.x / _meshSettings.MeshWorldSize * _chunkSize, worldPosition.z / _meshSettings.MeshWorldSize * _chunkSize, 0.0f);
        GameObject newMarker = SpawnMarker(sprite, markerPosition, _markerSize, false);
        _spawnedWorldMarkers.Add(newMarker);
    }

    private static GameObject SpawnMarker(Sprite sprite, Vector3 position, float size, bool raycastTarget)
    {
        GameObject newMarker = new GameObject(sprite.name);
        newMarker.transform.parent = _markersContainer;

        //Position
        newMarker.transform.localScale = Vector3.one;
        newMarker.transform.localPosition = position;

        //Add sprite
        Image image = newMarker.AddComponent<Image>();
        image.sprite = sprite;

        //Size
        image.rectTransform.sizeDelta = new Vector2(size, size);
        image.raycastTarget = raycastTarget;

        newMarker.SetActive(_displaying);

        _worldMarkers.Add(new WorldMarker(sprite, position, raycastTarget));

        return newMarker;
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

public struct TextureSave
{
    public Texture2D Texture { get; private set; }
    public Vector3 ChunkCoord { get; private set; }

    public TextureSave(Texture2D texture, Vector3 chunkCoord)
    {
        this.Texture = texture;
        this.ChunkCoord = chunkCoord;
    }
}

public struct WorldMarker
{
    public Sprite Sprite { get; private set; }
    public Vector3 LocalPosition { get; private set; }
    public bool Editable { get; private set; }

    public WorldMarker(Sprite sprite, Vector3 localPosition, bool editable)
    {
        this.Sprite = sprite;
        this.LocalPosition = localPosition;
        this.Editable = editable;
    }
}
