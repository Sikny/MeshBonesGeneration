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
        if(GUILayout.Button("G�n�rer le mod�le et squelette"))
        {
            ((GenerateSkeleton)target).Start();
        }
    }
}
