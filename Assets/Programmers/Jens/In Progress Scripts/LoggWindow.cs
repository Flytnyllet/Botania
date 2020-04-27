using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoggWindow : MonoBehaviour
{
    [SerializeField] Text _textObject;
    [SerializeField] Image _imageObject;
    public static string LOGG_WINDOW_EVENT = "LoggWindowAdd";

    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    public void AddWindowElement(string s)
    {
        GameObject gObject = Instantiate(_textObject.gameObject, this.transform);
        gObject.GetComponent<Text>().text = s;
    }
    public void AddWindowElement(Sprite s)
    {
        GameObject gObject = Instantiate(_textObject.gameObject, this.transform);
        gObject.GetComponent<Image>().sprite = s;
    }
}
