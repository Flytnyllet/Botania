using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InteractText : UIElement
{
    protected class NotificationObject
    {
        public Coroutine currentCoroutineText;
        public Coroutine currentCoroutineImage;
        public string name { get; set; }
        public Sprite sprite { get; set; }
    }
    protected NotificationObject _notifciationObj = new NotificationObject();
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
            if (_notifciationObj.currentCoroutineText != null)
                StopCoroutine(_notifciationObj.currentCoroutineText);

            _textPopUp.text = _text;
            _notifciationObj.currentCoroutineText = StartCoroutine(FadeOut(1, 2));
        }


        if (_imagePopUp != null)
        {
            if (_notifciationObj.currentCoroutineImage != null)
                StopCoroutine(_notifciationObj.currentCoroutineImage);

            _imagePopUp.sprite = sprite;
            _notifciationObj.currentCoroutineImage = StartCoroutine(FadeOut(1.5f, 2));
        }

    }
}
