using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;

        if (DrawDefaultInspector() && mapGenerator.DoAutoUpdate())
            mapGenerator.DrawMapInEditor();

        if (GUILayout.Button("Generate"))
            mapGenerator.DrawMapInEditor();
    }
}
