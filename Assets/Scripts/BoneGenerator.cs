using System;
using System.Collections.Generic;
using UnityEngine;

public class BoneGenerator : MonoBehaviour
{
    public Mesh sourceMesh;
    public GameObject vertexSpherePrefab;
    public Vector3 baryCenter;
    public Vector3 outBoneVector;


    public List<Vector3> projectedPoints = new List<Vector3>();

    private void Start()
    {
        GenerateBone();
    }

    [ContextMenu("Generate Bone")]
    public void GenerateBone()
    {
        // 1 - barycentre
        Vector3 g = Vector3.zero;
        Vector3[] vertices = sourceMesh.vertices;
        int vertexCount = vertices.Length;
        for (int i = vertexCount - 1; i >= 0; --i)
        {
            Instantiate(vertexSpherePrefab, vertices[i], Quaternion.identity);
            g += vertices[i];
        }
        g /= vertexCount;   // barycentre
        baryCenter = g;
        
        // 2 - mise a l origine du barycentre
        Vector3 og = g;
        for (int i = vertexCount - 1; i >= 0; i--)
        {
            vertices[i] -= og;
        }
        
        // 3 - calcul matrice de covariance M : 3x3
        var covarianceM = vertices.CovarianceMat();
        
        // 4 - algorithme de recherche de valeur propre dominante de M et de son vecteur propre v associé
        var eigenVec = covarianceM.GetEigenVector(100);
        //outBoneVector = eigenVec;

        // 5 - Projeter les points sur le vecteur propre
        Vector3 max, min;
        foreach(var point in vertices){
            projectedPoints.Add(point.Project(eigenVec));
        }
        max = projectedPoints[0];
        min = projectedPoints[0];
        foreach(var point in projectedPoints)
        {
            max = Vector3.Max(max, point);
            min = Vector3.Min(min, point);
        }
        Debug.Log("Max = " + max + ", min : " + min);

        // 6 - Repositionner chaque partie du mesh ainsi que chaque composante principale
        max = max + og;
        min = min + og;
        for (int i = vertexCount - 1; i >= 0; i--)
        {
            vertices[i] += og;
        }

        Instantiate(vertexSpherePrefab, max, Quaternion.identity);
        Instantiate(vertexSpherePrefab, min, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        if (sourceMesh == null) return;
        Gizmos.color = Color.green;
        for (int i = sourceMesh.vertices.Length - 1; i >= 0; i--)
        {
            Gizmos.DrawRay(baryCenter, outBoneVector/2);
        }
    }
}
