using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class BoneGenerator : MonoBehaviour
{
    public Mesh sourceMesh;
    public GameObject vertexSpherePrefab;
    public GameObject minMaxPrefab;
    public Vector3 baryCenter;
    public Vector3 outBoneVectorMax;
    public Vector3 outBoneVectorMin;
    public Material material;


    public List<Vector3> projectedPoints = new List<Vector3>();

    public void Init()
    {
        GenerateBone();

        gameObject.AddComponent<MeshFilter>().sharedMesh = sourceMesh;
        gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
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
            //Instantiate(vertexSpherePrefab, vertices[i], Quaternion.identity);
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
        var eigenVec = covarianceM.GetEigenVector(100).normalized;

        // 5 - Projeter les points sur le vecteur propre
        Vector3 max = Vector3.zero, min = Vector3.zero;
        foreach(var point in vertices){
            var minToMax = min - max;
            var projected = point.Project(eigenVec);
            projectedPoints.Add(projected);

            var projToMax = max - projected;
            var minToProj = projected - min;
            if (Vector3.Dot(eigenVec, projToMax) >= 0 && minToMax.magnitude < minToProj.magnitude)
            {
                max = projected;
                minToMax = min - max;
            }

            if (Vector3.Dot(eigenVec, minToProj) > 0 && minToMax.magnitude < projToMax.magnitude)
            {
                min = projected;
            }
        }
        
        // 6 - Repositionner chaque partie du mesh ainsi que chaque composante principale
        max += og;
        min += og;
        for (int i = vertexCount - 1; i >= 0; i--)
        {
            vertices[i] += og;
        }
/*
        Instantiate(minMaxPrefab, max, Quaternion.identity);
        Instantiate(minMaxPrefab, min, Quaternion.identity);
*/
        outBoneVectorMax = max;
        outBoneVectorMin = min;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        Gizmos.DrawLine(outBoneVectorMin, outBoneVectorMax);

        /*Handles.color = Color.magenta;
        Handles.DrawPolyLine(projectedPoints.Select(point => point + baryCenter).ToArray());*/
    }
}
