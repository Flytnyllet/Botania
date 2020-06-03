using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class NotificationObject
{
    public static Coroutine currentCoroutineText;
    public static Coroutine currentCoroutineImage;
    public static string name { get; set; }
    public static Sprite sprite { get; set; }
}

public class InteractText : UIElement
{
    protected Text _textPopUp;
    protected Image _imagePopUp;
    protected string _text;

    void Start()
    {
        if (GetComponent<Text>() != null)
        {
            element = GetComponent<Text>();
            _textPopUp = GetComponent<Text>();
        }

        if (GetComponent<Image>() != null)
        {
            element = GetComponent<Image>();
            _imagePopUp = GetComponent<Image>();
        }
        PickupFlower.onPickUpEvent += PopUp;
    }

    public virtual void PopUp(string _text = null, Sprite sprite = null)
    {
        if (_textPopUp != null)
        {
            if (NotificationObject.currentCoroutineText != null)
                StopCoroutine(NotificationObject.currentCoroutineText);

            _textPopUp.text = _text;
            NotificationObject.currentCoroutineText = StartCoroutine(FadeOut(1, 2));
        }


        if (_imagePopUp != null)
        {
            if (NotificationObject.currentCoroutineImage != null)
                StopCoroutine(NotificationObject.currentCoroutineImage);

            _imagePopUp.sprite = sprite;
            NotificationObject.currentCoroutineImage = StartCoroutine(FadeOut(1.5f, 2));
        }

    }
}
