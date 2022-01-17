using System.Collections.Generic;
using UnityEngine;

public class ShapeDetector : MonoBehaviour
{
    public MeshFilter meshFilter;
    public float radiusError = 0.1f;
    
    private struct Sphere
    {
        public float Radius;
        public Vector3 Center;
    }

    private List<Sphere> _spheres = new List<Sphere>();

    [ContextMenu("Detect Spheres")]
    private void DetectSpheres()
    {
        void CheckSphere(ref List<Vector3> pointsList)
        {
            Vector3 center = Vector3.zero;
            for (int i = pointsList.Count - 1; i >= 0; --i)
            {
                center += pointsList[i];
            }
            center /= pointsList.Count;
            float meanRadius = 0;
            for (int i = pointsList.Count - 1; i >= 0; --i)
            {
                meanRadius += Vector3.Distance(center, pointsList[i]);
            }
            meanRadius /= pointsList.Count;
            for (int i = pointsList.Count - 1; i >= 0; --i)
            {
                float dist = Vector3.Distance(center, pointsList[i]);
                if (Mathf.Abs(dist - meanRadius) > radiusError)
                {
                    pointsList.RemoveAt(i);
                    break;
                }
            }
        }

        var vertices = meshFilter.sharedMesh.vertices;
        var pointsList = new List<Vector3>();
        for (int i = vertices.Length - 1; i >= 0; i--)
        {
            pointsList.Add(vertices[i]);
            CheckSphere(ref pointsList);
        }
        Debug.Log(pointsList.Count);
        
        _spheres.Clear();
        Vector3 baryCenter = Vector3.zero;
        for (int i = pointsList.Count - 1; i >= 0; --i)
        {
            baryCenter += pointsList[i];
        }
        baryCenter /= pointsList.Count;
        float meanRadius = 0;
        for (int i = pointsList.Count - 1; i >= 0; --i)
        {
            meanRadius += Vector3.Distance(baryCenter, pointsList[i]);
        }
        meanRadius /= pointsList.Count;
        _spheres.Add(new Sphere{Center = baryCenter, Radius = meanRadius});
        Debug.Log("Sphere center : " + baryCenter + " ; radius : " + meanRadius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = _spheres.Count - 1; i >= 0; i--)
        {
            Gizmos.DrawWireSphere(_spheres[i].Center, _spheres[i].Radius);
        }
    }
}
