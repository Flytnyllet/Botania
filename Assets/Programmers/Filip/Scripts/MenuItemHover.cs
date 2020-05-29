using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class MenuItemHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] bool _changeNameOnClick = false;
    [SerializeField] string _newName = "Return to Game";
    [SerializeField] float _newWidth = 580f;

    [SerializeField] GameObject _hover;
    [SerializeField] GameObject _notHover;

    private void Awake()
    {
        _hover.SetActive(false);
        _notHover.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData data)
    {
        _notHover.SetActive(false);
        _hover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData data)
    {
        _notHover.SetActive(true);
        _hover.SetActive(false);
    }

    public void OnPointerDown(PointerEventData data)
    {
        _hover.SetActive(false);
        _notHover.SetActive(true);
    }

    public void ChangeName()
    {
        if (_changeNameOnClick)
        {
            _changeNameOnClick = false;
            _hover.GetComponent<TMP_Text>().text = _newName;
            _notHover.GetComponent<TMP_Text>().text = _newName;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(_newWidth, rectTransform.sizeDelta.y);
        }
    }
}
