using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
#if (UNITY_EDITOR) 
[CustomEditor(typeof(Amb_GetRandomEvent))]
[CanEditMultipleObjects]
public class Amb_GetRandomEventEditor : Editor
{
    SerializedProperty dataAsset;
    SerializedProperty dataList;

    string[] _lists = new[]
    {
        "Amb_Forest",
        "Amb_Grassland"
    };

    int _listIndex = 0;

    void OnEnable()
    {
        dataAsset = serializedObject.FindProperty("amb_Data");
        dataList = serializedObject.FindProperty("amb_List");
        _listIndex = Array.IndexOf(_lists, dataList.stringValue);

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(dataAsset, new GUIContent("Amb_Data_Asset"));

        _listIndex = EditorGUILayout.Popup("Amb_Local_List", _listIndex, _lists);
        if (_listIndex < 0)
            _listIndex = 0;
        dataList.stringValue = _lists[_listIndex];

        serializedObject.ApplyModifiedProperties();
    }
}
#endif