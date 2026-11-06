using System.Numerics;
using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.Surfaces
{
    public sealed class BezierSurface
    {
        public const int Degree = 3;

        public const int PointsPerSide = Degree + 1;

        public const int ControlPointCount = PointsPerSide * PointsPerSide;

        public Vector3[,] ControlPoints { get; }

        public Vector3[,] RotatedControlPoints { get; }

        public BezierSurface(IReadOnlyList<Vector3> pointsInFileOrder)
        {
            if (pointsInFileOrder.Count != ControlPointCount)
            {
                throw new ArgumentException(
                    $"A Bezier surface needs exactly {ControlPointCount} control points, got {pointsInFileOrder.Count}.");
            }

            ControlPoints = new Vector3[PointsPerSide, PointsPerSide];

            for (int k = 0; k < ControlPointCount; k++)
            {
                int i = k / PointsPerSide;
                int j = k % PointsPerSide;
                ControlPoints[i, j] = pointsInFileOrder[k];
            }

            RotatedControlPoints = (Vector3[,])ControlPoints.Clone();
        }

        public void ApplyRotation(SceneOrientation orientation)
        {
            for (int i = 0; i < PointsPerSide; i++)
            {
                for (int j = 0; j < PointsPerSide; j++)
                {
                    RotatedControlPoints[i, j] = orientation.RotatePoint(ControlPoints[i, j]);
                }
            }
        }

        public Vector3 Point(float u, float v)
        {
            Vector3 result = Vector3.Zero;

            for (int i = 0; i <= Degree; i++)
            {
                for (int j = 0; j <= Degree; j++)
                {
                    float weight = Bernstein.Polynomial(i, Degree, u) * Bernstein.Polynomial(j, Degree, v);
                    result += ControlPoints[i, j] * weight;
                }
            }

            return result;
        }

        public Vector3 TangentU(float u, float v)
        {
            Vector3 sum = Vector3.Zero;

            for (int i = 0; i <= Degree - 1; i++)
            {
                for (int j = 0; j <= Degree; j++)
                {
                    Vector3 difference = ControlPoints[i + 1, j] - ControlPoints[i, j];
                    float weight = Bernstein.Polynomial(i, Degree - 1, u) * Bernstein.Polynomial(j, Degree, v);
                    sum += difference * weight;
                }
            }

            return Degree * sum;
        }

        public Vector3 TangentV(float u, float v)
        {
            Vector3 sum = Vector3.Zero;

            for (int i = 0; i <= Degree; i++)
            {
                for (int j = 0; j <= Degree - 1; j++)
                {
                    Vector3 difference = ControlPoints[i, j + 1] - ControlPoints[i, j];
                    float weight = Bernstein.Polynomial(i, Degree, u) * Bernstein.Polynomial(j, Degree - 1, v);
                    sum += difference * weight;
                }
            }

            return Degree * sum;
        }
    }
}
