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

    Animator _animator;
    bool _doAnimationOnAwake = false;
    string _trigger;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (_doAnimationOnAwake)
            _animator.SetTrigger(_trigger);

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
        _usingInputField = _inputField.isFocused;
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

    public void Setup(MapMarkers type, string name, float size)
    {
        _inputField.text = name;

        if (_animator == null)
        {
            _doAnimationOnAwake = true;
            _trigger = type.ToString();
        }
        else
            _animator.SetTrigger(type.ToString());

        _image.raycastTarget = true;
        GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
    }

    public void ToggleMovement(bool status)
    {
        if (status)
            CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU_NO_MOVEMENT);
        else
            CharacterState.SetControlState(CHARACTER_CONTROL_STATE.MENU);
    }
}
