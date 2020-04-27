using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviour
{
    [SerializeField] protected MaskableGraphic element;

    protected IEnumerator FadeOut(float time)
    {
        Color color = Color.white;
        float alphaChange = 1 / time;
        time += Time.time;
        while (time > Time.time)
        {
            time += Time.deltaTime;
            color.a -= alphaChange * Time.deltaTime;
            element.color = color;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
