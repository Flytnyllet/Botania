using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnStartFadeIn : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] AnimationCurve _curve;
    [SerializeField, Range(0.01f, 20)] float _speed;

    [Header("Drop")]

    [SerializeField] Image _image;

    Timer _fadeTimer;
    Color _startColor;

    private void Awake()
    {
        _fadeTimer = new Timer(_speed);
        _startColor = _image.color;
        Color color = _startColor;
        color.a = 0.0f;
        _image.color = color;
    }

    private void Update()
    {
        _fadeTimer.Time += Time.deltaTime;

        if (!_fadeTimer.Expired())
        {
            float point = _curve.Evaluate(_fadeTimer.Ratio());
            Color newColor = _startColor;
            newColor.a = point;
            _image.color = newColor;
        }
        else
            Destroy(this.gameObject);
    }
}
