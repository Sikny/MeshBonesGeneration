using System;
using UnityEngine;

public class BoneGenerator : MonoBehaviour
{
    public Mesh sourceMesh;

    [ContextMenu("Generate Bone")]
    public void GenerateBone()
    {
        
    }

    private void OnDrawGizmos()
    {
        /*if (sourceMesh == null) return;
        Gizmos.color = Color.blue;
        for (int i = sourceMesh.vertices.Length - 1; i >= 0; i--)
        {
            Gizmos.DrawSphere(sourceMesh.vertices[i], 0.001f);
        }*/
    }
}
