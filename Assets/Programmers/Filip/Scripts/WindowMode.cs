using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowMode : MonoBehaviour
{
    [SerializeField] GameObject _selection;

    private void OnEnable()
    {
        _selection.SetActive(!Screen.fullScreen);
    }

    public void ChangeWindowMode()
    {
        bool newMode = _selection.activeSelf;

        Screen.fullScreen = newMode;

        ChangeSelection();
    }

    void ChangeSelection()
    {
        _selection.SetActive(!_selection.activeSelf);
    }
}
