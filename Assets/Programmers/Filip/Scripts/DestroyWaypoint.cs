using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyWaypoint : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] RectTransform _spriteTransform;
    [SerializeField] AnimationCurve _curve;
    [SerializeField, Range(0.01f, 2f)] float _plopTime = 0.5f;
    [SerializeField, Range(0.0f, 1.0f)] float _lableSize = 0.25f;

    Timer _plopTimer;

    private void Awake()
    {
        _plopTimer = new Timer(_plopTime);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            MapGenerator.RemoveWaypoint(transform.parent.transform);
            StartCoroutine(RemoveWaypoint());
        }
    }

    IEnumerator RemoveWaypoint()
    {
        while (!_plopTimer.Expired())
        {
            _plopTimer.Time += Time.deltaTime;
            float scale = _curve.Evaluate(_plopTimer.Ratio());
            Vector3 newScale = new Vector3(scale, scale, scale);
            _spriteTransform.localScale = newScale;
            transform.localScale = newScale * _lableSize;

            yield return new WaitForEndOfFrame();
        }

        Destroy(transform.parent.gameObject);
    }
}
