namespace BezierSurfaces.Mathematics
{
    // Wielomiany Bernsteina
    public static class Bernstein
    {
        // Waga i-tego punktu kontrolnego dla parametru t przy stopniu n.
        public static float Polynomial(int i, int n, float t)
        {
            return BinomialCoefficient(n, i) * MathF.Pow(t, i) * MathF.Pow(1 - t, n - i);
        }

        // Dwumianowa
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
