using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternativeInteractText : InteractText
{
    public override void PopUp(string _text = null, Sprite sprite = null)
    {
        if (_textPopUp != null)
        {
            if (_notifciationObj.currentCoroutineText != null)
                StopCoroutine(_notifciationObj.currentCoroutineText);

            _notifciationObj.currentCoroutineText = StartCoroutine(FadeOut(1, 2));
        }


        if (_imagePopUp != null)
        {
            if (_notifciationObj.currentCoroutineImage != null)
                StopCoroutine(_notifciationObj.currentCoroutineImage);

            _notifciationObj.currentCoroutineImage = StartCoroutine(FadeOut(1.5f, 2));
        }

    }
}
