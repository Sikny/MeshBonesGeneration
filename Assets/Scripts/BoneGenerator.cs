using System;
using UnityEngine;

public class BoneGenerator : MonoBehaviour
{
    public Mesh sourceMesh;
    public GameObject vertexSpherePrefab;
    public Vector3 baryCenter;
    public Vector3 outBoneVector;

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
        outBoneVector = eigenVec;

        // 5 - 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = sourceMesh.vertices.Length - 1; i >= 0; i--)
        {
            Gizmos.DrawRay(baryCenter, outBoneVector/2);
        }
    }
}
