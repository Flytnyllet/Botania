using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Map : MonoBehaviour, IDragHandler, IScrollHandler
{
    [Header("Drop")]

    [SerializeField] Canvas _parentCanvas;
    [SerializeField] RectTransform _spawnContainer;
    [SerializeField] RectTransform _pivotSpawnContainer;
    //[SerializeField] Image

    [Header("Settings")]

    [SerializeField, Range(0.01f, 20)] float _dragSpeed = 1;
    [SerializeField, Range(0, 4)] float _zoomInPercentage = 1.33f;
    [SerializeField, Range(0, 1)] float _zoomOutPercentage = 0.75f;

    [SerializeField, Range(0, 15)] float _maxScale = 7.5f;
    [SerializeField, Range(0, 15)] float _minScale = 0.5f;

    Transform _viewer;
    List<GameObject> _spawnedMapChunks;
    bool _displaying = false;

    public RectTransform SpawnContainer { get { return _spawnContainer; } private set { _spawnContainer = value; } }

    private void Awake()
    {
        _spawnedMapChunks = new List<GameObject>();
    }

    private void Start()
    {
        _viewer = Player.GetPlayerTransform();
    }

    private void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        _spawnContainer.anchoredPosition += eventData.delta * _dragSpeed / _pivotSpawnContainer.localScale.x / _parentCanvas.scaleFactor;
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 oldPosition = _spawnContainer.position;
        _pivotSpawnContainer.position = eventData.position;

        float newScale;

        if (eventData.scrollDelta.y > 0)
            newScale = _pivotSpawnContainer.localScale.x * _zoomInPercentage;
        else
            newScale = _pivotSpawnContainer.localScale.x * _zoomOutPercentage;

        newScale = Mathf.Clamp(newScale, _minScale, _maxScale);

        _spawnContainer.position = oldPosition;

        _pivotSpawnContainer.localScale = new Vector3(newScale, newScale, newScale);
    }

    public void AddSpawnedChunk(GameObject newChunk)
    {
        newChunk.SetActive(_displaying);
        _spawnedMapChunks.Add(newChunk);
    }

    public void Display(bool status)
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
