namespace BezierSurfaces.Mathematics
{
    public static class Bernstein
    {
        public static float Polynomial(int i, int n, float t)
        {
            return BinomialCoefficient(n, i) * MathF.Pow(t, i) * MathF.Pow(1 - t, n - i);
        }

        public static float BinomialCoefficient(int n, int k)
        {
            float result = 1;
            for (int step = 1; step <= k; step++)
            {
                result = result * (n - k + step) / step;
            }

            return result;
        }
    }
}
