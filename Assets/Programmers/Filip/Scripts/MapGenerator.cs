using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public enum MapMarkers
{
    X,
    FLOWER,
    SWOOP
}

[System.Serializable]
public struct MapMarkerDropDownSetup
{
    public MapMarkers _type;
    public Sprite _sprite;
}

public class MapGenerator : MonoBehaviour, IDragHandler, IScrollHandler, IPointerDownHandler
{
    static MapGenerator _singleton;

    [Header("Drop")]

    [SerializeField] MapSettings _mapSettings;
    [SerializeField] MeshSettings _meshSettings;
    [SerializeField] GameObject _waypointPrefab;
    [SerializeField] GameObject _markerPrefab;

    [Header("Setup")]

    [SerializeField] Canvas _parentCanvas;
    [SerializeField] RectTransform _mapHolder;
    [SerializeField] RectTransform _spawnContainer;
    [SerializeField] RectTransform _pivotSpawnContainer;
    [SerializeField] RectTransform _markersContainer;
    [SerializeField] RectTransform _waypointContainer;
    [SerializeField] GameObject _enableDisableObject;
    [SerializeField] RectTransform _objectsRectTransform;
    [SerializeField] Image _waypointSelectedImage;
    [SerializeField] Image _playerIcon;
    [SerializeField] Image _raycastMask;
    [SerializeField] Dropdown _waypointsDropdown;

    [SerializeField] AudioSource _waypointDestroyAudioSource;

    [Header("Marker Sprites")]

    [SerializeField] List<MapMarkerDropDownSetup> _mapMakerDropDownSetupList;

    [Header("General Settings")]

    [SerializeField, Range(1, 100)] float _chunkSize = 20;
    [SerializeField, Range(1, 100)] float _markerSize = 5;
    [SerializeField, Range(1, 100)] float _waypointSize = 10f;
    [SerializeField, Range(0.01f, 100)] float _playerIconSize = 8;
    [SerializeField, Range(0.01f, 10)] float _centerMapScale = 2.5f;
    [SerializeField, Range(0.01f, 5)] float _timePressTwiceToLockCenter = 0.5f;
    [SerializeField] string _standardWayPointName = "New Waypoint";

    [SerializeField, Range(0.01f, 20)] float _dragSpeed = 1;
    [SerializeField, Range(0, 4)] float _zoomInPercentage = 1.33f;
    [SerializeField, Range(0, 1)] float _zoomOutPercentage = 0.75f;

    [SerializeField, Range(0, 15)] float _maxScale = 7.5f;
    [SerializeField, Range(0, 15)] float _minScale = 0.5f;

    static Dictionary<Vector2, TextureSave> _renderedMapChunks;
    static Dictionary<Vector3, WorldMarker> _worldMarkers;
    static Transform _viewer;
    static List<GameObject> _spawnedMapChunks;
    static List<Transform> _spawnedWorldMarkers;
    static List<Transform> _spawnedWaypoints;
    static bool _displaying = true;
    static Vector3 _centerMapScaleVector;
    static Timer _centerTwiceTimer;
    static bool _lockedCenter = false;
    static bool _justPressedCenter = false;

    static MapMarkers _currentSelectedWaypoint;
    RectTransform _canvasRectTransform;

    private void Awake()
    {
        if (_singleton == null)
        {
            _singleton = this;

            _spawnedMapChunks = new List<GameObject>();
            _spawnedWorldMarkers = new List<Transform>();
            _spawnedWaypoints = new List<Transform>();
            _renderedMapChunks = new Dictionary<Vector2, TextureSave>();
            _worldMarkers = new Dictionary<Vector3, WorldMarker>();

            _centerMapScaleVector = new Vector3(_centerMapScale, _centerMapScale, _centerMapScale);

            _centerTwiceTimer = new Timer(_timePressTwiceToLockCenter);
        }
        else
            Destroy(_parentCanvas.gameObject);
    }

    private void Start()
    {
        _parentCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		_canvasRectTransform = _parentCanvas.GetComponent<RectTransform>();

		_viewer = Player.GetPlayerTransform();
        UpdateCurrentWaypoint();
        Display(false);
    }

    private void Update()
    {
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

    //adds options in dropdown - waypoints
    void SetupDropdownOptions()
    {
        List<Sprite> spriteList = new List<Sprite>();

        for (int i = 0; i < _mapMakerDropDownSetupList.Count; i++)
        {
            spriteList.Add(_mapMakerDropDownSetupList[i]._sprite);
        }

        _waypointsDropdown.AddOptions(spriteList);
    }

    //If placing waypoint right now, this is the sprite to use
    public void UpdateCurrentWaypoint()
    {
        _currentSelectedWaypoint = (MapMarkers)_waypointsDropdown.value;
    }

    private static Sprite GetSpriteByEnum(MapMarkers type)
    {
        for (int i = 0; i < _singleton._mapMakerDropDownSetupList.Count; i++)
        {
            if (_singleton._mapMakerDropDownSetupList[i]._type == type)
                return _singleton._mapMakerDropDownSetupList[i]._sprite;
        }

        return _singleton._mapMakerDropDownSetupList[0]._sprite;
    }

    #region Save and Load
    public static void Save()
    {
        List<TextureSave> textureSaves = _renderedMapChunks.Select(value => value.Value).ToList();
        List<WorldMarker> markerSaves = _worldMarkers.Select(value => value.Value).ToList();

        Serialization.Save(Saving.FileNames.MAP_TEXTURES, textureSaves);
        Serialization.Save(Saving.FileNames.MAP_MARKERS, markerSaves);
    }

    public static void Load()
    {
        List<TextureSave> textureSaves = (List<TextureSave>)Serialization.Load(Saving.FileNames.MAP_TEXTURES);
        List<WorldMarker> markerSaves = (List<WorldMarker>)Serialization.Load(Saving.FileNames.MAP_MARKERS);

        if (textureSaves != null)
            for (int i = 0; i < textureSaves.Count; i++)
                AddChunkToMap(textureSaves[i].ChunkCoord);

        if (markerSaves != null)
        {
            for (int i = 0; i < markerSaves.Count; i++)
            {
                if (markerSaves[i].IsWaypoint)
                    AddWaypoint(markerSaves[i].LocalPosition, markerSaves[i].Name, markerSaves[i].Type);
                //else
                //    AddWorldMarkerLocal((MapMarkers)markerSaves[i].Index, markerSaves[i].LocalPosition, markerSaves[i].Name);
            }
        }
    }

    public static void Wipe()
    {
        List<TextureSave> textureSaves = new List<TextureSave>();
        List<WorldMarker> markerSaves = new List<WorldMarker>();

        Serialization.Save(Saving.FileNames.MAP_TEXTURES, textureSaves);
        Serialization.Save(Saving.FileNames.MAP_MARKERS, markerSaves);
    }
    #endregion

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
        _singleton._pivotSpawnContainer.localPosition = Vector3.zero;
        _singleton._mapHolder.localPosition = -_singleton._playerIcon.rectTransform.localPosition;

        _singleton._pivotSpawnContainer.localScale = _centerMapScaleVector;
        ScaleMarkersByPivot();
    }

    //If real player moves, move it on map
    private static void UpdatePlayerIcon()
    {
        Vector3 newPosition = new Vector3(_viewer.position.x / _singleton._meshSettings.MeshWorldSize * _singleton._chunkSize, _viewer.position.z / _singleton._meshSettings.MeshWorldSize * _singleton._chunkSize, 0.0f);
        _singleton._playerIcon.rectTransform.anchoredPosition = newPosition;
        _singleton._playerIcon.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -_viewer.rotation.eulerAngles.y);

        float scale = 1 / _singleton._pivotSpawnContainer.localScale.x * _singleton._playerIconSize;
        Vector3 playerScale = new Vector3(scale, scale, scale);

        _singleton._playerIcon.transform.localScale = playerScale;
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
        float markerScaleF = 1 / _singleton._pivotSpawnContainer.localScale.x * _singleton._markerSize;
        Vector3 markerScale = new Vector3(markerScaleF, markerScaleF, markerScaleF);

        for (int i = 0; i < _spawnedWorldMarkers.Count; i++)
        {
            _spawnedWorldMarkers[i].localScale = markerScale;
        }

        float waypointScaleF = 1 / _singleton._pivotSpawnContainer.localScale.x * _singleton._waypointSize;
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
        Vector2 sampleCenter = chunkCoord * _singleton._meshSettings.MeshWorldSize / _singleton._meshSettings.MeshScale;

        ThreadedDataRequester.RequestData(() => RequestTextureChunkData(chunkCoord, sampleCenter), ReceivedTextureChunkData);
    }

    //Threaded work
    private static TextureChunkData RequestTextureChunkData(Vector2 chunkCoord, Vector2 sampleCenter)
    {
        return TextureGenerator.DrawMap(_singleton._meshSettings.NumVertsPerLine, _singleton._mapSettings, sampleCenter, chunkCoord, 1);
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
        {
            _renderedMapChunks.Add(chunkCoord, new TextureSave(chunkCoord));
            InstantiateChunk(texture, chunkCoord);
        }
        else
            Debug.LogWarning("This terrainchunk is already a key for a texture? Multiple callings? Or reading savings?");
    }

    private static void InstantiateChunk(Texture2D texture, Vector2 chunkCoord)
    {
        //Create GameObject
        GameObject mapChunk = new GameObject(chunkCoord.ToString());
        mapChunk.transform.parent = _singleton._spawnContainer;
        mapChunk.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90f);

        //Position
        mapChunk.transform.localScale = Vector3.one;
        mapChunk.transform.localPosition = chunkCoord * _singleton._chunkSize;

        //Add Texture
        RawImage image = mapChunk.AddComponent<RawImage>();
        image.texture = texture;
        image.rectTransform.sizeDelta = new Vector2(_singleton._chunkSize, _singleton._chunkSize);
        image.raycastTarget = false;

        _spawnedMapChunks.Add(mapChunk);
    }
    #endregion

    #region Waypoints and Markers

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Position
            float x = (eventData.position.x / Screen.width - 0.5f) * _canvasRectTransform.sizeDelta.x / _pivotSpawnContainer.localScale.x;
            float y = (eventData.position.y / Screen.height - 0.5f) * _canvasRectTransform.sizeDelta.y / _pivotSpawnContainer.localScale.y;
            Vector3 waypointPosition = new Vector3(x, y, 0.0f) - _objectsRectTransform.localPosition / _pivotSpawnContainer.localScale.x - _mapHolder.localPosition - _pivotSpawnContainer.localPosition / _pivotSpawnContainer.localScale.x;

            AddWaypoint(waypointPosition, _standardWayPointName, _currentSelectedWaypoint);
        }
    }

    private static void AddWaypoint(Vector3 position, string name, MapMarkers type)
    {
        //Scale
        float scale = 1 / _singleton._pivotSpawnContainer.localScale.x * _singleton._waypointSize;
        Vector3 waypointScale = new Vector3(scale, scale, scale);

        GameObject newWaypoint = Instantiate(_singleton._waypointPrefab, Vector3.zero, Quaternion.identity, _singleton._waypointContainer);
        newWaypoint.transform.localPosition = position;
        newWaypoint.transform.localScale = waypointScale;

        WaypointMarker script = newWaypoint.GetComponent<WaypointMarker>();

        script.Setup(type, name, _singleton._waypointSize);

        _worldMarkers.Add(position, new WorldMarker(position, true, WaypointMarker.STANDARD_WAYPOINT_NAME, type));

        _spawnedWaypoints.Add(newWaypoint.transform);
    }

    //public static void AddWorldMarkerGlobal(MapMarkers mapMarkerType, Vector3 worldPosition, string name)
    //{
    //    Vector3 markerPosition = new Vector3(worldPosition.x / _singleton._meshSettings.MeshWorldSize * _singleton._chunkSize, worldPosition.z / _singleton._meshSettings.MeshWorldSize * _singleton._chunkSize, 0.0f);

    //    AddWorldMarkerLocal(mapMarkerType, markerPosition, name);
    //}

    //private static void AddWorldMarkerLocal(MapMarkers mapMarkerType, Vector3 localPosition, string name)
    //{
    //    //This marker is already loaded in from memory
    //    if (_worldMarkers.ContainsKey(localPosition))
    //        return;

    //    //Scale
    //    float scale = 1 / _singleton._pivotSpawnContainer.localScale.x * _singleton._markerSize;
    //    Vector3 markerScale = new Vector3(scale, scale, scale);

    //    GameObject newMarker = SpawnMarker(mapMarkerType, localPosition, markerScale, _singleton._markerSize, name);
    //    _spawnedWorldMarkers.Add(newMarker.transform);
    //}

    //private static GameObject SpawnMarker(MapMarkers mapMarkerType, Vector3 position, Vector3 scale, float size, string name)
    //{
    //    GameObject newMarker = Instantiate(_singleton._markerPrefab, Vector3.zero, Quaternion.identity, _singleton._markersContainer);
    //    newMarker.name = name;

    //    newMarker.transform.localPosition = position;
    //    newMarker.transform.localScale = scale;

    //    newMarker.GetComponent<MarkerAnimation>().Setup((int)mapMarkerType);

    //    Image image = newMarker.GetComponentInChildren<Image>();

    //    image.rectTransform.sizeDelta = new Vector2(size, size);
    //    image.raycastTarget = false;

    //    _worldMarkers.Add(position, new WorldMarker(position, false, name, mapMarkerType));

    //    return newMarker;
    //}

    //Used only for saving the name of waypoints
    public static void WaypointNameChange(Transform waypoint, string newName)
    {
        if (_worldMarkers.ContainsKey(waypoint.localPosition))
        {
            WorldMarker worldMarker = _worldMarkers[waypoint.localPosition];
            worldMarker.SetName(newName);
            _worldMarkers[waypoint.localPosition] = worldMarker;
        }
    }

    public static void RemoveWaypoint(Transform waypoint)
    {
        if (_singleton._waypointDestroyAudioSource != null)
            _singleton._waypointDestroyAudioSource.Play();

        _spawnedWaypoints.Remove(waypoint);
        _worldMarkers.Remove(waypoint.localPosition);
    }
    #endregion

    //Show or don't show map
    public static void Display(bool status)
    {
        if (_displaying != status)
        {
            _displaying = status;

            _singleton._raycastMask.raycastTarget = status;
            _singleton._enableDisableObject.SetActive(status);

            if (status)
            {
                //CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
                UpdatePlayerIcon();
                FocusOnPlayer();
            }
            //else
            //    CharacterState.SetControlState(CHARACTER_CONTROL_STATE.PLAYERCONTROLLED);*/
        }
    }
}

#region Save Structs

//These structs are only used to store saving info
[System.Serializable]
public struct TextureSave
{
    public Vector3 ChunkCoord { get; private set; }

    public TextureSave(Vector3 chunkCoord)
    {
        this.ChunkCoord = chunkCoord;
    }
}

[System.Serializable]
public struct WorldMarker
{
    public MapMarkers Type { get; private set; }
    public Vector3 LocalPosition { get; private set; }
    public bool IsWaypoint { get; private set; }
    public string Name { get; private set; }

    public WorldMarker(Vector3 localPosition, bool isWaypoint, string name, MapMarkers type = MapMarkers.X)
    {
        this.Type = type;
        this.LocalPosition = localPosition;
        this.IsWaypoint = isWaypoint;
        this.Name = name;
    }

    public void SetName(string newName)
    {
        Name = newName;
    }
}
#endregion