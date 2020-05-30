using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnStartFadeIn : MonoBehaviour
{
    static OnStartFadeIn _thisSingleton;

    [Header("Settings")]

    [SerializeField] AnimationCurve _fadeInCurve;
    [SerializeField] AnimationCurve _fadeOutCurve;
    [SerializeField, Range(0.01f, 20)] float _fadeInSpeed = 6;
    [SerializeField, Range(0.01f, 20)] float _fadeOutSpeed = 3; 
    [SerializeField] bool _destroyOnUse = true;

    [Header("Drop")]

    [SerializeField] Image _image;

    AnimationCurve _curve;
    bool _toggle = false;
    bool _fadeIn = true;
    Timer _fadeTimer;
    Color _startColor;

    private void Awake()
    {
        if (_thisSingleton == null)
            _thisSingleton = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        _startColor = _image.color;
    }

    public static void FadeIn()
    {
        _thisSingleton._curve = _thisSingleton._fadeInCurve;
        _thisSingleton._fadeTimer = new Timer(_thisSingleton._fadeInSpeed);
        _thisSingleton._fadeIn = true;
        _thisSingleton._toggle = true;
    }

    public static void FadeOut()
    {
        _thisSingleton._curve = _thisSingleton._fadeOutCurve;
        _thisSingleton._fadeTimer = new Timer(_thisSingleton._fadeOutSpeed);
        _thisSingleton._fadeIn = false;
        _thisSingleton._toggle = true;
    }

    private void Update()
    {
        if (_toggle)
            Fade();
    }

    public static bool Done()
    {
        return !_thisSingleton._toggle;
    }

    void Fade()
    {
        _fadeTimer.Time += Time.deltaTime;

        if (!_fadeTimer.Expired())
        {
            float point = _curve.Evaluate(_fadeTimer.Ratio());
            Color newColor = _startColor;
            newColor.a = point * _startColor.a;
            _image.color = newColor;
        }
        else
        {
            Color finalColor = _startColor;
            finalColor.a = _fadeIn ? 0.0f : 1.0f;
            _image.color = finalColor;

            if (_destroyOnUse)
                Destroy(this.gameObject);

            _toggle = false;
        } 
    }
}
