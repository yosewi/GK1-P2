using System.Numerics;

namespace BezierSurfaces.Mathematics
{
    // Współrzędne barycentryczne: trzy wagi mówiące, jak blisko punkt jest każdego wierzchołka trójkąta.
    // Służą do przeliczania wartości z wierzchołków (np. głębokości z) na dowolny piksel wewnątrz trójkąta.
    public sealed class BarycentricCoordinates
    {
        // Wierzchołki trójkąta na ekranie i jego podwojone pole.
        private readonly Vector2 _a;
        private readonly Vector2 _b;
        private readonly Vector2 _c;
        private readonly float _doubleArea;

        public BarycentricCoordinates(Vector2 a, Vector2 b, Vector2 c)
        {
            _a = a;
            _b = b;
            _c = c;
            _doubleArea = DoubleSignedArea(a, b, c);
        }

        // Trójkąt o zerowym polu
        public bool IsDegenerate => _doubleArea == 0;

        // Wagi punktu (x, y) dla wierzchołków A, B i C. Każda waga to stosunek pola małego trójkąta
        // leżącego naprzeciw danego wierzchołka do pola całego trójkąta.
        public Vector3 Compute(float x, float y)
        {
            Vector2 point = new Vector2(x, y);

            float weightA = DoubleSignedArea(point, _b, _c) / _doubleArea;
            float weightB = DoubleSignedArea(_a, point, _c) / _doubleArea;
            float weightC = 1 - weightA - weightB;

            return new Vector3(weightA, weightB, weightC);
        }

        // Średnia ważona trzech wartości z wierzchołków, czyli wartość w punkcie wewnątrz trójkąta.
        public static float Interpolate(Vector3 weights, float valueA, float valueB, float valueC)
        {
            return weights.X * valueA + weights.Y * valueB + weights.Z * valueC;
        }

        // To samo dla wektorów, np. wektorów normalnych w wierzchołkach.
        public static Vector3 Interpolate(Vector3 weights, Vector3 valueA, Vector3 valueB, Vector3 valueC)
        {
            return weights.X * valueA + weights.Y * valueB + weights.Z * valueC;
        }

        // Podwojone pole trójkąta ze znakiem (znak zależy od kolejności wierzchołków).
        private static float DoubleSignedArea(Vector2 p, Vector2 q, Vector2 r)
        {
            return (q.X - p.X) * (r.Y - p.Y) - (r.X - p.X) * (q.Y - p.Y);
        }
    }
}
