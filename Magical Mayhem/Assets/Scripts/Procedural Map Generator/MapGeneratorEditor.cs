using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator)), CanEditMultipleObjects]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
        base.OnInspectorGUI();

        MapGenerator mapGenerator = (MapGenerator)target;
        if(GUILayout.Button("Generate")){
            mapGenerator.GenerateMap(1);
        }
        if(GUILayout.Button("Delete")){
            mapGenerator.ResetMap();
        }
    }
}