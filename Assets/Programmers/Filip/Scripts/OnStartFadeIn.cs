using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnStartFadeIn : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] AnimationCurve _curve;
    [SerializeField, Range(0.01f, 20)] float _speed;
    [SerializeField] bool _destroyOnUse = true;

    [Header("Drop")]

    [SerializeField] Image _image;

    bool toggle = false;
    Timer _fadeTimer;
    Color _startColor;

    private void Awake()
    {
        _fadeTimer = new Timer(_speed);
    }

    private void OnEnable()
    {
        _startColor = _image.color;
        Color color = _startColor;
        color.a = 0.0f;
        _image.color = color;
        toggle = true;
        _fadeTimer.Reset();
    }

    private void Update()
    {
        if (toggle)
        {
            _fadeTimer.Time += Time.deltaTime;

            if (!_fadeTimer.Expired())
            {
                float point = _curve.Evaluate(_fadeTimer.Ratio());
                Color newColor = _startColor;
                newColor.a = point * _startColor.a;
                _image.color = newColor;
            }
            else if (_destroyOnUse)
                Destroy(this.gameObject);
            else
                toggle = false;
        }
    }
}
