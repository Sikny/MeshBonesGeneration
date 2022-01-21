using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor (typeof(GenerateSkeleton))]
public class SpawnSkeleton: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Générer le modèle et squelette"))
        {
            ((GenerateSkeleton)target).Start();
        }
    }
}
