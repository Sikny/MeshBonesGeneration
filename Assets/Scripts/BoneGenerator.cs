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

        // 5 - Projeter les points sur le vecteur propre
        Vector3? max = null, min = null;
        foreach(var point in vertices){
            max ??= point;
            min ??= point;
            Vector3 minToMax = min.Value - max.Value;
            Vector3 projected = point.Project(eigenVec);

            var projToMax = max.Value - projected;
            var minToProj = projected - min.Value;
            if (minToMax.magnitude < minToProj.magnitude
                && (Vector3.Dot(minToMax, projToMax) >= 0 || minToMax == Vector3.zero))
            {
                max = projected;
                minToMax = min.Value - max.Value;
            }

            if (minToMax.magnitude < projToMax.magnitude
                && (Vector3.Dot(minToMax, minToProj) > 0 || minToMax == Vector3.zero))
            {
                min = projected;
            }
            
            projectedPoints.Add(projected);
        }
        
        // 6 - Repositionner chaque partie du mesh ainsi que chaque composante principale
        max += og;
        min += og;
        for (int i = vertexCount - 1; i >= 0; i--)
        {
            vertices[i] += og;
        }

        Instantiate(minMaxPrefab, max.Value, Quaternion.identity);
        Instantiate(minMaxPrefab, min.Value, Quaternion.identity);

        outBoneVectorMax = max.Value;
        outBoneVectorMin = min.Value;
    }

    private void OnDrawGizmos()
    {
        if (sourceMesh == null) return;
        Gizmos.color = Color.green;
        for (int i = sourceMesh.vertices.Length - 1; i >= 0; i--)
        {
            Gizmos.DrawLine(outBoneVectorMin, outBoneVectorMax);
        }

        Handles.color = Color.magenta;
        Handles.DrawPolyLine(projectedPoints.Select(point => point + baryCenter).ToArray());
    }
}
