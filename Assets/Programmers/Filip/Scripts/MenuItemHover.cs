using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuItemHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool _changeNameOnClick = false;
    [SerializeField] Image[] _text;
    [SerializeField] Sprite[] _newSprite;
    [SerializeField] RectTransform _targetRectTransform;
    [SerializeField] float _newTargetWidth = 580f;

    [SerializeField] GameObject _hover;

    private void OnEnable()
    {
        _hover.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData data)
    {
        _hover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData data)
    {
        _hover.SetActive(false);
    }

    public void ChangeName()
    {
        if (_changeNameOnClick)
        {
            _changeNameOnClick = false;

            for (int i = 0; i < _text.Length; i++)
            {
                _text[i].sprite = _newSprite[i];
                _targetRectTransform.sizeDelta = new Vector2(_newTargetWidth, _targetRectTransform.sizeDelta.y);
            }
        }
    }
}
