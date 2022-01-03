using System.Collections.Generic;
using UnityEngine;

public class SeparateBodyMeshParts : MonoBehaviour
{
    public MeshFilter meshFilter;
    public float errorParam = 0.1f;
    public bool showVertices;
    public bool showWireFrame;

    private Vector3[] _vertices;
    private void Awake()
    {
        Mesh sourceMesh = meshFilter.sharedMesh;
        List<MeshFilter> outMeshes = new List<MeshFilter>();

        _vertices = sourceMesh.vertices;
    }

    private void OnDrawGizmos() {
        if (showWireFrame) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(meshFilter.sharedMesh);
        }

        if (!showVertices) return;
        Gizmos.color = Color.black;
        var vertices = meshFilter.sharedMesh.vertices;
        for (int i = vertices.Length - 1; i >= 0; i--)
        {
            Vector3 vertex = vertices[i];
            Gizmos.DrawSphere(vertex, 0.0025f);
        }
    }
}
