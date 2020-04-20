using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class WaypointMarker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]

    [SerializeField, Range(0, 300)] float _minTextWidth = 30;
    [SerializeField, Range(0, 300)] float _textHeight = 30;

    [Header("Setup")]

    [SerializeField] RectTransform _inputFieldRectTransform;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] TMP_Text _text;

    Image _image;
    RectTransform _rectTransform;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        FormatText();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Select(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Select(false);
    }

    private void FormatText()
    {
        _inputFieldRectTransform.sizeDelta = new Vector2((_text.renderedWidth > 0 ? _text.renderedWidth : 0) + _minTextWidth, _textHeight);
        _text.rectTransform.localPosition = Vector2.zero;
        transform.GetComponentInChildren<TMP_SelectionCaret>().rectTransform.localPosition = Vector2.zero;
    }

    public void Select(bool status)
    {
        _text.gameObject.SetActive(status);

        if (status)
            _inputField.image.color = Color.white;
        else
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void NameChange()
    {
        //MapGenerator.
    }

    public void Setup(Sprite sprite, float size)
    {
        _image.sprite = sprite;
        _image.raycastTarget = true;
        _rectTransform.sizeDelta = new Vector2(size, size);

        _inputField.Select();
        FormatText();
    }
}
