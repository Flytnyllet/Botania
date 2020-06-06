using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrickDen : MonoBehaviour
{
    [SerializeField] GameObject _focus;

    public void ChangeActive()
    {
        _focus.SetActive(!_focus.activeSelf);
    }
}
