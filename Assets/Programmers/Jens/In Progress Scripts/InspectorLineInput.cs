using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(LineInput))]
public class InspectorLineInput : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LineInput myScript = (LineInput)target;
        if (GUILayout.Button("Build Object"))
        {
            myScript.Engage();
        }
    }
}
