using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviour
{
    [SerializeField] protected MaskableGraphic element;

    protected IEnumerator FadeOut(float time, float fadeStart)
    {
        float temp = 0;
        Color color = Color.white;
        element.color = color;
        float alphaChange = 1 / time;

        while (temp < fadeStart)
        {
            temp += Time.deltaTime;
            yield return null;
        }

        time += Time.time;
        while (time > Time.time)
        {
            //time += Time.deltaTime;
            color.a -= alphaChange * Time.deltaTime;
            element.color = color;
            yield return null;
        }
        //Destroy(this.gameObject);
    }
}