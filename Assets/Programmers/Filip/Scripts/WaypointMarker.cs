using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class WaypointMarker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static readonly string STANDARD_WAYPOINT_NAME = "New Waypoint";
	static bool _usingInputField = false;

    [Header("Settings")]

    [SerializeField, Range(0, 300)] float _minTextWidth = 30;
    [SerializeField, Range(0, 300)] float _textHeight = 30;

    [Header("Setup")]

    [SerializeField] RectTransform _inputFieldRectTransform;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] TMP_Text _text;
    [SerializeField] Image _image;

    RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        Select(true);
        Select(false);
    }

    private void Update()
    {
        FormatText();
		UsingInputField();
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

	void UsingInputField()
	{
		if(_inputField.isFocused)
		{
			_usingInputField = true;
		}
		else
		{
			_usingInputField = false;
		}
	}

	public static bool InputFieldFocus()
	{
		return _usingInputField;
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
        MapGenerator.WaypointNameChange(transform, _inputField.text);
    }

    public void Setup(Sprite sprite, string name, float size)
    {
        _inputField.text = name;
        _image.sprite = sprite;
        _image.raycastTarget = true;
        _rectTransform.sizeDelta = new Vector2(size, size);

        FormatText();
    }

    public void ToggleMovement(bool status)
    {
        if (status)
            CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU_NO_MOVEMENT);
        else
            CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
    }
}
