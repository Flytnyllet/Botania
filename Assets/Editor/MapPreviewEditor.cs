using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(MapPreview))]
public class MapPreviewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapPreview mapPreview = (MapPreview)target;

        if (DrawDefaultInspector() && mapPreview.DoAutoUpdate())
            mapPreview.DrawMapInEditor();

        if (GUILayout.Button("Generate"))
            mapPreview.DrawMapInEditor();
    }
}
