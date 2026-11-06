using System.Numerics;

namespace BezierSurfaces.Mathematics
{
    public sealed class BarycentricCoordinates
    {
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

        public bool IsDegenerate => _doubleArea == 0;

        public Vector3 Compute(float x, float y)
        {
            Vector2 point = new Vector2(x, y);

            float weightA = DoubleSignedArea(point, _b, _c) / _doubleArea;
            float weightB = DoubleSignedArea(_a, point, _c) / _doubleArea;
            float weightC = 1 - weightA - weightB;

            return new Vector3(weightA, weightB, weightC);
        }

        public static float Interpolate(Vector3 weights, float valueA, float valueB, float valueC)
        {
            return weights.X * valueA + weights.Y * valueB + weights.Z * valueC;
        }

        private static float DoubleSignedArea(Vector2 p, Vector2 q, Vector2 r)
        {
            return (q.X - p.X) * (r.Y - p.Y) - (r.X - p.X) * (q.Y - p.Y);
        }
    }
}
