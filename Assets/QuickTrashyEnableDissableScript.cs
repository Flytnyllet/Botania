using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTrashyEnableDissableScript : MonoBehaviour
{
    [SerializeField] GameObject[] _target;
    private void OnEnable()
    {
        EventManager.Subscribe(EventNameLibrary.OPEN_BOOK, SetEnabled);
        EventManager.Subscribe(EventNameLibrary.CLOSE_BOOK, SetDissabled);
    }
    private void OnDisable()
    {
        EventManager.UnSubscribe(EventNameLibrary.OPEN_BOOK, SetEnabled);
        EventManager.UnSubscribe(EventNameLibrary.CLOSE_BOOK, SetDissabled);
    }
    void SetEnabled(EventParameter param = null)
    {
        foreach (GameObject gObject in _target)
        {
            gObject.SetActive(true);
        }
    }
    void SetDissabled(EventParameter param = null)
    {
        foreach (GameObject gObject in _target)
        {
            gObject.SetActive(false);
        }
    }
}
