using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//[CustomEditor(typeof(LineInput))]
public class InspectorLineInput : EditorWindow
{
    [SerializeField] string _input;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Input")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(InspectorLineInput));
    }

    void OnGUI()
    {
        EditorGUILayout.TextField("Text Field", _input);
        if (GUILayout.Button("Build Object"))
            {
                //myScript.Engage();
                GUI.FocusControl(null);
                Repaint();
            }
    }


    //public override void OnInspectorGUI()
    //{
    //    DrawDefaultInspector();

    //    LineInput myScript = (LineInput)target;
    //    if (GUILayout.Button("Build Object"))
    //    {
    //        myScript.Engage();
    //        GUI.FocusControl(null);
    //        Repaint();
    //    }
    //}

}
