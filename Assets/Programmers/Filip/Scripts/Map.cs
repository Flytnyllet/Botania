using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour, IDragHandler, IScrollHandler
{
    [Header("Drop")]

    [SerializeField] Canvas _parentCanvas;
    [SerializeField] RectTransform _spawnContainer;


    [Header("Settings")]

    [SerializeField, Range(0.01f, 20)] float _dragSpeed = 1;
    [SerializeField, Range(0, 4)] float _zoomInPercentage = 1.33f;
    [SerializeField, Range(0, 1)] float _zoomOutPercentage = 0.75f;

    public RectTransform SpawnContainer { get { return _spawnContainer; } private set { _spawnContainer = value; } }

    public float ContainerScale()
    {
        return _spawnContainer.localScale.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _spawnContainer.anchoredPosition += eventData.delta * _dragSpeed / _parentCanvas.scaleFactor;
    }

    public void OnScroll(PointerEventData eventData)
    {
        float newScale;

        if (eventData.scrollDelta.y > 0)
            newScale = _spawnContainer.localScale.x * _zoomInPercentage;
        else
            newScale = _spawnContainer.localScale.x * _zoomOutPercentage;

        _spawnContainer.localScale = new Vector3(newScale, newScale, newScale);
    }
}
