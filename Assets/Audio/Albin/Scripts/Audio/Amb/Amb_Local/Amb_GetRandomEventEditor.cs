using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
#if (UNITY_EDITOR) 
[CustomEditor(typeof(Amb_GetRandomEvent))]
[CanEditMultipleObjects]
public class Amb_GetRandomEventEditor : Editor
{
    SerializedProperty ambLocalWind;
    SerializedProperty ambIsShy;
    SerializedProperty ambList;

    string[] _lists = new[]
    {
        "Amb_Forest",
        "Amb_Grassland"
    };

    int _listIndex = 0;

    void OnEnable()
    {
        ambLocalWind = serializedObject.FindProperty("amb_AttachLocalWind");
        ambIsShy = serializedObject.FindProperty("amb_ShyBehaviour");
        ambList = serializedObject.FindProperty("amb_List");
        _listIndex = Array.IndexOf(_lists, ambList.stringValue);

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(ambLocalWind, new GUIContent("Amb_AttachLocalWind"));
        EditorGUILayout.PropertyField(ambIsShy, new GUIContent("Amb_IsShyBehaviour"));

        _listIndex = EditorGUILayout.Popup("Amb_Local_List", _listIndex, _lists);
        if (_listIndex < 0)
            _listIndex = 0;
        ambList.stringValue = _lists[_listIndex];

        serializedObject.ApplyModifiedProperties();
    }
}
#endif