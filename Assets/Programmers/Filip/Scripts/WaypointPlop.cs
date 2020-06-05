using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointPlop : MonoBehaviour
{

    [SerializeField] AnimationCurve _sizeCurve;
    [SerializeField] AnimationCurve _alphaCurve;
    [SerializeField] RectTransform _labelTransform;
    [SerializeField] Image _spriteImage;
    [SerializeField] Image _labelImage;
    [SerializeField, Range(0.01f, 2f)] float _plopTime = 0.5f;
    [SerializeField, Range(0, 10)] float _alphaTime = 2.0f;
    [SerializeField, Range(0.0f, 1.0f)] float _lableSize = 0.25f;

    Timer _plopTimer;
    Timer _alphaTimer;
    //Vector3 _spriteStartScale;
    Vector3 _labelStartScale;
    //Color _spriteStartColor;
    Color _labelStartColor;

    private void Awake()
    {
        _plopTimer = new Timer(_plopTime);
        _alphaTimer = new Timer(_alphaTime);
        //_spriteStartScale = transform.localScale;
        //_spriteStartColor = _spriteImage.color;

        //Color startColorSprite = _spriteStartColor;
        //startColorSprite.a = 0.0f;
        //_spriteImage.color = startColorSprite;

        //So world markers can use script
        if (_labelTransform != null)
        {
            _labelStartScale = _labelTransform.localScale;
            _labelStartColor = _labelImage.color;

            Color startColorLabel = _labelStartColor;
            startColorLabel.a = 0.0f;
            _labelImage.color = startColorLabel;
        }
    }

    void Update()
    {
        _plopTimer.Time += Time.deltaTime;
        _alphaTimer.Time += Time.deltaTime;

        if (!_plopTimer.Expired())
        {
            float point = _sizeCurve.Evaluate(_plopTimer.Ratio());

            Vector3 newScale = new Vector3(point, point, point);
            //transform.localScale = newScale;

            //So world markers can use script
            if (_labelTransform != null)
                _labelTransform.localScale = newScale * _lableSize;
        }
        if (!_alphaTimer.Expired())
        {
            float point = _alphaCurve.Evaluate(_alphaTimer.Ratio());

            //Color spriteImageColor = _spriteImage.color;
            //spriteImageColor.a = point;
            //_spriteImage.color = spriteImageColor;

            //So world markers can use script
            if (_labelTransform != null)
            {
                Color labelImageColor = _labelImage.color;
                labelImageColor.a = point;
                _labelImage.color = labelImageColor;
            }
        }
        if (_alphaTimer.Expired() && _plopTimer.Expired())
        {
            //transform.localScale = _spriteStartScale;
            //_spriteImage.color = _spriteStartColor;
            //So world markers can use script
            if (_labelTransform != null)
            {
                _labelImage.color = _labelStartColor;
                _labelTransform.localScale = _labelStartScale;
            }
            Destroy(this);
        }
    }
}
