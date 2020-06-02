using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowMode : MonoBehaviour
{
    [SerializeField] GameObject _selection;

    bool _first = true;

    private void OnEnable()
    {
        if (!_first)
            _selection.SetActive(!Screen.fullScreen);
    }

    private void Start()
    {
        if (_first)
        {
            _first = false;
            _selection.SetActive(!Screen.fullScreen);
        }
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
