using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LineInput : MonoBehaviour
{
    [SerializeField] string _input;

    public void Engage()
    {
        _input = "";
        //Repaint();
    }
}
