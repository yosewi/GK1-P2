namespace WinFormsApp1.Mathematics
{
    /// <summary>
    /// Wielomiany Bernsteina - "wagi" punktów kontrolnych w krzywych i powierzchniach Béziera:
    ///
    ///     B(i, n, t) = C(n, i) · t^i · (1 − t)^(n − i),      t ∈ [0, 1],  i = 0..n
    ///
    /// gdzie C(n, i) to dwumian Newtona ("n po i").
    /// Dla każdego t wagi B(0, n, t) ... B(n, n, t) sumują się do 1.
    /// </summary>
    public static class Bernstein
    {
        public static float Polynomial(int i, int n, float t)
        {
            return BinomialCoefficient(n, i) * MathF.Pow(t, i) * MathF.Pow(1 - t, n - i);
        }

        /// <summary>
        /// Dwumian Newtona C(n, k) = n! / (k! · (n − k)!), liczony iloczynem bez silni:
        /// C(n, k) = (n / 1) · ((n − 1) / 2) · ... · ((n − k + 1) / k).
        /// Działa dla dowolnego n (przyda się też przy krzywej światła stopnia 4-5).
        /// </summary>
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
