using System.Numerics;
using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.Surfaces
{
    // Powierzchnia Béziera opisana siatką 4x4 punktów kontrolnych.
    public sealed class BezierSurface
    {
        // Stopień 3 w obu kierunkach, czyli 4 punkty na bok i 16 punktów kontrolnych
        public const int Degree = 3;

        public const int PointsPerSide = Degree + 1;

        public const int ControlPointCount = PointsPerSide * PointsPerSide;

        // Punkty kontrolne przed obrotem
        public Vector3[,] ControlPoints { get; }

        // Te same punkty po obrocie
        public Vector3[,] RotatedControlPoints { get; }

        // Układa 16 punktów z pliku w siatkę 4x4
        public BezierSurface(IReadOnlyList<Vector3> pointsInFileOrder)
        {
            ControlPoints = new Vector3[PointsPerSide, PointsPerSide];

            for (int k = 0; k < ControlPointCount; k++)
            {
                int i = k / PointsPerSide;
                int j = k % PointsPerSide;
                ControlPoints[i, j] = pointsInFileOrder[k];
            }

            // Dopóki nie było obrotu, wersja po obrocie jest kopią oryginału.
            RotatedControlPoints = (Vector3[,])ControlPoints.Clone();
        }

        // Obraca punkty kontrolne bieżącym obrotem sceny. Zawsze zaczyna od oryginalnych punktów
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

        // Punkt powierzchni dla parametrów u i v z przedziału od 0 do 1.
        // To średnia ważona wszystkich 16 punktów kontrolnych z wagami z wielomianów Bernsteina.
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

        // Wektor styczny w kierunku u: w którą stronę przesuwa się punkt powierzchni, gdy rośnie u.
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

        // Wektor styczny w kierunku v, liczony tak samo z różnic sąsiednich punktów w kierunku v.
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
