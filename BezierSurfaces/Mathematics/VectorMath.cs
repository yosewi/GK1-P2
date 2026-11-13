using System.Numerics;

namespace BezierSurfaces.Mathematics
{
    // Pomocnicze operacje na wektorach.
    public static class VectorMath
    {
        private const float MinLength = 1e-6f;

        // Skraca wektor do długości 1. Gdy wektor jest prawie zerowy
        // zwraca wektor skierowany do obserwatora zamiast dzielić przez zero.
        public static Vector3 SafeNormalize(Vector3 vector)
        {
            return vector.Length() < MinLength ? Vector3.UnitZ : Vector3.Normalize(vector);
        }
    }
}