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
    static RectTransform __waypointContainer;
    static RectTransform _pivotSpawnContainer;
    static RectTransform _mapHolder;
    static GameObject _enableDisableObject;
    static Image _playerIcon;
    static GameObject _markerPrefab;

    static AudioSource _waypointDestroyAudioSource;

    static float _chunkSize;
    static float _markerSize;
    static float _waypointSize;
    static float _playerIconSize;

    [Header("Drop")]

    [SerializeField] MapSettings _mapSettingsInstance;
    [SerializeField] MeshSettings _meshSettingsInstance;
    [SerializeField] GameObject _waypointPrefab;
    [SerializeField] GameObject _markerPrefabInstance;

    [Header("Setup")]

    [SerializeField] Canvas _parentCanvas;
    [SerializeField] RectTransform _mapHolderInstance;
    [SerializeField] RectTransform _spawnContainerInstance;
    [SerializeField] RectTransform _pivotSpawnContainerInstance;
    [SerializeField] RectTransform _markersContainerInstance;
    [SerializeField] RectTransform _waypointContainerInstance;
    [SerializeField] GameObject _enableDisableObjectInstance;
    [SerializeField] Image _waypointSelectedImage;
    [SerializeField] Image _playerIconInstance;

    [SerializeField] AudioSource _waypointDestroyAudioSourceInstance;

    [Header("General Settings")]

    [SerializeField, Range(1, 100)] float _chunkSizeInstance = 20;
    [SerializeField, Range(1, 100)] float _markerSizeInstance = 5;
    [SerializeField, Range(1, 100)] float _waypointSizeInstance = 7.5f;
    [SerializeField, Range(0.01f, 100)] float _playerIconSizeInstance = 8;
    [SerializeField, Range(0.01f, 10)] float _centerMapScale = 2.5f;
    [SerializeField, Range(0.01f, 5)] float _timePressTwiceToLockCenter = 0.5f;

    [SerializeField, Range(0.01f, 20)] float _dragSpeed = 1;
    [SerializeField, Range(0, 4)] float _zoomInPercentage = 1.33f;
    [SerializeField, Range(0, 1)] float _zoomOutPercentage = 0.75f;

    [SerializeField, Range(0, 15)] float _maxScale = 7.5f;
    [SerializeField, Range(0, 15)] float _minScale = 0.5f;

    static Dictionary<Vector2, TextureSave> _renderedMapChunks;
    static Dictionary<Transform, WorldMarker> _worldMarkers;
    static Transform _viewer;
    static List<GameObject> _spawnedMapChunks;
    static List<Transform> _spawnedWorldMarkers;
    static List<Transform> _spawnedWaypoints;
    static bool _displaying = false;
    static Vector3 _centerMapScaleVector;
    static Timer _centerTwiceTimer;
    static bool _lockedCenter = false;
    static bool _justPressedCenter = false;

    Sprite _waypoint;
    RectTransform _canvasRectTransform;

    private void Awake()
    {
        if (_singletonMapGenerator == null)
        {
            _singletonMapGenerator = this;

            _spawnedMapChunks = new List<GameObject>();
            _spawnedWorldMarkers = new List<Transform>();
            _spawnedWaypoints = new List<Transform>();
            _renderedMapChunks = new Dictionary<Vector2, TextureSave>();
            _worldMarkers = new Dictionary<Transform, WorldMarker>();

            _meshSettings = _meshSettingsInstance;
            _mapSettings = _mapSettingsInstance;
            _chunkSize = _chunkSizeInstance;
            _spawnContainer = _spawnContainerInstance;
            _markersContainer = _markersContainerInstance;
            __waypointContainer = _waypointContainerInstance;
            _pivotSpawnContainer = _pivotSpawnContainerInstance;
            _markerSize = _markerSizeInstance;
            _waypointSize = _waypointSizeInstance;
            _enableDisableObject = _enableDisableObjectInstance;
            _mapHolder = _mapHolderInstance;
            _playerIcon = _playerIconInstance;
            _playerIconSize = _playerIconSizeInstance;
            _markerPrefab = _markerPrefabInstance;


            _waypointDestroyAudioSource = _waypointDestroyAudioSourceInstance;

            _canvasRectTransform = _parentCanvas.GetComponent<RectTransform>();

            _centerMapScaleVector = new Vector3(_centerMapScale, _centerMapScale, _centerMapScale);

            _centerTwiceTimer = new Timer(_timePressTwiceToLockCenter);
        }
        else
            Destroy(_parentCanvas.gameObject);
    }

    private void Start()
    {
        _viewer = Player.GetPlayerTransform();
        UpdateWaypointSprite();

        Display(true); //ONLY TESTING
    }

    private void Update()
    {
        //Only for testing
        if (Input.GetKeyDown(KeyCode.M))
        {
            Display(!_displaying);
        }

        if (_displaying)
        {
            if (_justPressedCenter && !_lockedCenter)
            {
                _centerTwiceTimer.Time += Time.deltaTime;

                if (_centerTwiceTimer.Expired())
                    _justPressedCenter = false;
            }

            if (_lockedCenter)
                FocusOnPlayer();

            UpdatePlayerIcon();
        }
    }

    //public void Save()
    //{
    //    Serialization.Save("TESTTESTETSASDASD", _canvasRectTransform);
    //    Serialization.Load("TESTTESTETSASDASD");
    //}

    //If placing waypoint right now, this is the sprite to use
    public void UpdateWaypointSprite()
    {
        _waypoint = _waypointSelectedImage.sprite;
    }

    //If real player moves, move it on map
    private static void UpdatePlayerIcon()
    {
        Vector3 newPosition = new Vector3(_viewer.position.x / _meshSettings.MeshWorldSize * _chunkSize, _viewer.position.z / _meshSettings.MeshWorldSize * _chunkSize, 0.0f);
        _playerIcon.rectTransform.anchoredPosition = newPosition;
        _playerIcon.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -_viewer.rotation.eulerAngles.y);

        float scale = 1 / _pivotSpawnContainer.localScale.x * _playerIconSize;
        Vector3 playerScale = new Vector3(scale, scale, scale);

        _playerIcon.transform.localScale = playerScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Position
            float x = (eventData.position.x / Screen.width - 0.5f) * _canvasRectTransform.sizeDelta.x / _pivotSpawnContainer.localScale.x;
            float y = (eventData.position.y / Screen.height - 0.5f) * _canvasRectTransform.sizeDelta.y / _pivotSpawnContainer.localScale.y;
            Vector3 waypointPosition = new Vector3(x, y, 0.0f) - _mapHolder.localPosition - _pivotSpawnContainer.localPosition / _pivotSpawnContainer.localScale.x;

            //Scale
            float scale = 1 / _pivotSpawnContainer.localScale.x * _waypointSize;
            Vector3 waypointScale = new Vector3(scale, scale, scale);

            GameObject newWaypoint = Instantiate(_waypointPrefab, Vector3.zero, Quaternion.identity, __waypointContainer);
            newWaypoint.transform.localPosition = waypointPosition;
            newWaypoint.transform.localScale = waypointScale;

            WaypointMarker script = newWaypoint.GetComponent<WaypointMarker>();
            script.Setup(_waypoint, _waypointSize);

            _worldMarkers.Add(newWaypoint.transform, new WorldMarker(_waypoint, waypointPosition, true, WaypointMarker.STANDARD_WAYPOINT_NAME));

            _spawnedWaypoints.Add(newWaypoint.transform);
        }
    }

    #region Map Manipulation
    //Used only for when the button is used to press for center
    public void FocusOnPlayerButton()
    {
        if (!_justPressedCenter && !_lockedCenter)
            _justPressedCenter = true;
        else if (_justPressedCenter && !_lockedCenter)
            _lockedCenter = true;
        else if (_lockedCenter)
            _lockedCenter = false;

        _centerTwiceTimer.Reset();
        FocusOnPlayer();
    }

    public static void FocusOnPlayer()
    {
        _pivotSpawnContainer.localPosition = Vector3.zero;
        _mapHolder.localPosition = -_playerIcon.rectTransform.localPosition;

        _pivotSpawnContainer.localScale = _centerMapScaleVector;
        ScaleMarkersByPivot();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
        {
            _lockedCenter = false;
            _mapHolder.anchoredPosition += eventData.delta * _dragSpeed / _pivotSpawnContainer.localScale.x / _parentCanvas.scaleFactor;
        }
    }

    //Make sure everything remains correct on zoom in and out around pivot
    public void OnScroll(PointerEventData eventData)
    {
        _lockedCenter = false;

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

        ScaleMarkersByPivot();
    }

    private static void ScaleMarkersByPivot()
    {
        float markerScaleF = 1 / _pivotSpawnContainer.localScale.x * _markerSize;
        Vector3 markerScale = new Vector3(markerScaleF, markerScaleF, markerScaleF);

        for (int i = 0; i < _spawnedWorldMarkers.Count; i++)
        {
            _spawnedWorldMarkers[i].localScale = markerScale;
        }

        float waypointScaleF = 1 / _pivotSpawnContainer.localScale.x * _waypointSize;
        Vector3 waypointScale = new Vector3(waypointScaleF, waypointScaleF, waypointScaleF);

        for (int i = 0; i < _spawnedWaypoints.Count; i++)
        {
            _spawnedWaypoints[i].localScale = waypointScale;
        }
    }

    #endregion

    #region Add texture chunk to map
    public static void AddChunkToMap(Vector2 chunkCoord)
    {
        Vector2 sampleCenter = chunkCoord * _meshSettings.MeshWorldSize / _meshSettings.MeshScale;

        ThreadedDataRequester.RequestData(() => RequestTextureChunkData(chunkCoord, sampleCenter), ReceivedTextureChunkData);
    }

    //Threaded work
    private static TextureChunkData RequestTextureChunkData(Vector2 chunkCoord, Vector2 sampleCenter)
    {
        return TextureGenerator.DrawMap(_meshSettings.NumVertsPerLine, _mapSettings, sampleCenter, chunkCoord, 1);
    }

    //Connect back with main thread
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
            Debug.LogWarning("This terrainchunk is already a key for a texture? Multiple callings? Or reading savings?");

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

        _spawnedMapChunks.Add(mapChunk);
    }
    #endregion

    public static void AddWorldMarker(Sprite sprite, Vector3 worldPosition)
    {
        Vector3 markerPosition = new Vector3(worldPosition.x / _meshSettings.MeshWorldSize * _chunkSize, worldPosition.z / _meshSettings.MeshWorldSize * _chunkSize, 0.0f);

        //Scale
        float scale = 1 / _pivotSpawnContainer.localScale.x * _markerSize;
        Vector3 markerScale = new Vector3(scale, scale, scale);

        GameObject newMarker = SpawnMarker(sprite, markerPosition, markerScale, _markerSize);
        _spawnedWorldMarkers.Add(newMarker.transform);
    }

    private static GameObject SpawnMarker(Sprite sprite, Vector3 position, Vector3 scale, float size)
    {
        GameObject newMarker = Instantiate(_markerPrefab, Vector3.zero, Quaternion.identity, _markersContainer);
        newMarker.name = sprite.name;

        newMarker.transform.localPosition = position;
        newMarker.transform.localScale = scale;

        Image image = newMarker.GetComponentInChildren<Image>();
        image.sprite = sprite;

        image.rectTransform.sizeDelta = new Vector2(size, size);
        image.raycastTarget = false;

        _worldMarkers.Add(newMarker.transform, new WorldMarker(sprite, position, false));

        return newMarker;
    }

    //Used only for saving the name of waypoints
    public static void WaypointNameChange(Transform waypoint, string newName)
    {
        if (_worldMarkers.ContainsKey(waypoint))
            _worldMarkers[waypoint].SetName(newName);
    }

    public static void RemoveWaypoint(Transform waypoint)
    {
        if (_waypointDestroyAudioSource != null)
            _waypointDestroyAudioSource.Play();

        _spawnedWaypoints.Remove(waypoint);
    }


    //Show or don't show map
    public static void Display(bool status)
    {
        if (_displaying != status)
        {
            _displaying = status;

            _enableDisableObject.SetActive(status);

            if (status)
            {
                CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
                UpdatePlayerIcon();
                FocusOnPlayer();
            }
            else
                CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);
        }
    }
}

//These structs are only used to store saving info
[System.Serializable]
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

[System.Serializable]
public struct WorldMarker
{
    public Sprite Sprite { get; private set; }
    public Vector3 LocalPosition { get; private set; }
    public bool Editable { get; private set; }
    public string Name { get; private set; }

    public WorldMarker(Sprite sprite, Vector3 localPosition, bool editable, string name = "")
    {
        this.Sprite = sprite;
        this.LocalPosition = localPosition;
        this.Editable = editable;
        this.Name = name;
    }

    public void SetName(string newName)
    {
        Name = newName;
    }
}
