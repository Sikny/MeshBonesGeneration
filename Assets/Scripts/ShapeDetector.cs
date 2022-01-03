using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeDetector : MonoBehaviour {
    private class Sphere {
        public Vector3 Center;
        public float Radius;
    }
    public float errorParam = 0.1f;
    public float centerMinDistance = 0.1f;
    public int minVerticesCountForShape = 10;
    public MeshFilter meshFilter;
    private List<Sphere> _spheres = new List<Sphere>();

    [ContextMenu("Detect Spheres")]
    private void DetectSpheres() {
        _spheres.Clear();
        Vector3[] vertices = meshFilter.sharedMesh.vertices;
        int verticesCount = vertices.Length;
        List<Vector3> spherePoints = new List<Vector3>();
        for (int i = 0; i < verticesCount; ++i) {
            Vector3 currentCenter = vertices[i];
            spherePoints.Clear();

            for (int j = 0; j < verticesCount; ++j) {
                if (j == i) continue;
                Vector3 currentVertex = vertices[j];
                float dist = Vector3.Distance(currentCenter, currentVertex);
                if (dist < errorParam) {
                    spherePoints.Add(currentVertex);
                }
            }

            if (spherePoints.Count >= minVerticesCountForShape) {
                currentCenter = Vector3.zero;
                foreach (var spherePoint in spherePoints) {
                    currentCenter += spherePoint;
                }

                currentCenter /= spherePoints.Count;
                // check center is not too close to another
                bool validCenter = true;
                for (int k = _spheres.Count - 1; k >= 0; k--) {
                    if (Vector3.Distance(_spheres[k].Center, currentCenter) < centerMinDistance) {
                        validCenter = false;
                        break;
                    }
                }
                if (!validCenter) continue;
                
                float radius = 0;
                for (int k = spherePoints.Count - 1; k >= 0; k--) {
                    radius += Vector3.Distance(spherePoints[k], currentCenter);
                }
                radius /= spherePoints.Count;
                Debug.Log("Added Sphere with center " + currentCenter + " and radius " + radius + " ; vertex count = " + spherePoints.Count);
                _spheres.Add(new Sphere {Center = currentCenter, Radius = radius});
            }
        }
        Debug.Log("Sphere count : " + _spheres.Count);
    }

    private void OnDrawGizmos() {
        if (_spheres.Count > 0) {
            Gizmos.color = Color.green;
            foreach (var sphere in _spheres) {
                Gizmos.DrawWireSphere(sphere.Center, sphere.Radius);
            }
        }
    }
}
