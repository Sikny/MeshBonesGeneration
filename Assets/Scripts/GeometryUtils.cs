using System.Linq;
using UnityEngine;

public static class GeometryUtils
{
    public static float[][] CovarianceMat(this Vector3[] values)
    {
        var xList = values.Select(pos => pos.x).ToArray();
        var yList = values.Select(pos => pos.y).ToArray();
        var zList = values.Select(pos => pos.z).ToArray();
        return new[]
        {
            new [] {xList.Variance(), Covariance(xList, yList), Covariance(xList, zList)},
            new [] {Covariance(yList, xList), yList.Variance(), Covariance(yList, zList)},
            new [] {Covariance(zList, xList), Covariance(zList, yList), zList.Variance()},
        };
    }

    public static float Covariance(float[] v1, float[] v2)
    {
        return v1.Zip(v2, (a, b) => a * b).Sum() / v1.Length - v1.Mean() * v2.Mean();
    }
    
    public static float Variance(this float[] values)
    {
        float mean = values.Mean();
        return values.Sum(v => v * v)/values.Length - mean * mean;
    }

    public static float Mean(this float[] values)
    {
        return values.Sum() / values.Length;
    }

    public static Vector3 GetEigenVector(this float[][] matrix, int iterations)
    {
        // 1 - vecteur initial v0 dont la plus grande composante est 1
        Vector3 v0 = Vector3.forward;
        
        // 2
        Vector3 vK = v0;
        for (int k = 0; k < iterations; ++k)
        {
            var prod = matrix.Multiply(vK);
            var lambdaK = prod.GetGreatestAbsoluteValue();
            vK = 1 / lambdaK * prod;
        }
        
        // 3
        return vK;
    }

    public static float GetGreatestAbsoluteValue(this Vector3 value)
    {
        var absX = Mathf.Abs(value.x);
        var absY = Mathf.Abs(value.y);
        var absZ = Mathf.Abs(value.z);

        if (absX < absY)
        {
            if (absY < absZ) return value.z;

            return value.y;
        }
        if (absX < absZ) return value.z;

        return value.x;
    }

    public static Vector3 Multiply(this float[][] matrix, Vector3 vector)
    {
        return new Vector3(
            matrix[0][0] * vector.x + matrix[0][1] * vector.y + matrix[0][2] * vector.z,
            matrix[1][0] * vector.x + matrix[1][1] * vector.y + matrix[1][2] * vector.z,
            matrix[2][0] * vector.x + matrix[2][1] * vector.y + matrix[2][2] * vector.z
        );
    }
}