using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPlop : MonoBehaviour
{
    [SerializeField] AnimationCurve _curve;
    [SerializeField] RectTransform _labelTransform;
    [SerializeField, Range(0.01f, 2f)] float _plopTime = 0.5f;
    [SerializeField, Range(0.0f, 1.0f)] float _lableSize = 0.25f;

    Timer _plopTimer;
    Vector3 _spriteStartScale;
    Vector3 _labelStartScale;

    private void Awake()
    {
        _plopTimer = new Timer(_plopTime);
        _spriteStartScale = transform.localScale;
        //So world markers can use script
        if (_labelTransform != null)
            _labelStartScale = _labelTransform.localScale;
    }

    void Update()
    {
        _plopTimer.Time += Time.deltaTime;

        float point = _curve.Evaluate(_plopTimer.Ratio());
        Vector3 newScale = new Vector3(point, point, point);
        transform.localScale = newScale;
        //So world markers can use script
        if (_labelTransform != null)
            _labelTransform.localScale = newScale * _lableSize;

        if (_plopTimer.Expired())
        {
            transform.localScale = _spriteStartScale;
            //So world markers can use script
            if (_labelTransform != null)
                _labelTransform.localScale = _labelStartScale;
            Destroy(this);
        }
    }
}
