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
        EventManager.Subscribe(LOGG_WINDOW_EVENT, AddWindowElement);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(LOGG_WINDOW_EVENT, AddWindowElement);
    }
    public void AddWindowElement(EventParameter param)
    {

    }
}
