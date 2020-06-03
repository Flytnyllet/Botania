using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternativeInteractText : InteractText
{
    public override void PopUp(string _text = null, Sprite sprite = null)
    {
        if (_textPopUp != null)
        {
            if (NotificationObject.currentCoroutineText != null)
                StopCoroutine(NotificationObject.currentCoroutineText);

            NotificationObject.currentCoroutineText = StartCoroutine(FadeOut(1, 2));
        }


        if (_imagePopUp != null)
        {
            if (NotificationObject.currentCoroutineImage != null)
                StopCoroutine(NotificationObject.currentCoroutineImage);

            NotificationObject.currentCoroutineImage = StartCoroutine(FadeOut(1.5f, 2));
        }

    }
}
