using System.Numerics;

namespace BezierSurfaces.Features.Lighting
{
    // Model oświetlenia.
    public sealed class LightingModel
    {
        public const float DefaultKd = 0.5f;
        public const float DefaultKs = 0.5f;
        public const int DefaultM = 20;

        private static readonly Vector3 ViewerDirection = Vector3.UnitZ;

        public float Kd { get; set; } = DefaultKd;

        public float Ks { get; set; } = DefaultKs;

        public int M { get; set; } = DefaultM;

        public Vector3 LightColor { get; set; } = Vector3.One;

        // Kolor punktu powierzchni o danym wektorze normalnym, oświetlonego z danego kierunku.
        public Vector3 Compute(Vector3 normal, Vector3 toLight, Vector3 objectColor)
        {
            float normalDotLight = Vector3.Dot(normal, toLight); // cos(N, L)

            float diffuse = MathF.Max(0, normalDotLight);

            // Kierunek, w którym światło odbija się od powierzchni.
            Vector3 reflected = 2 * normalDotLight * normal - toLight;

            // Składowa zwierciadlana
            float specular = MathF.Pow(MathF.Max(0, Vector3.Dot(ViewerDirection, reflected)), M); // cos^m(V,R)

            Vector3 lightTimesObject = LightColor * objectColor;
            return Kd * diffuse * lightTimesObject + Ks * specular * lightTimesObject;
        }

        public static Color ToColor(Vector3 color)
        {
            return Color.FromArgb(ToByte(color.X), ToByte(color.Y), ToByte(color.Z));
        }

        public static Vector3 FromColor(Color color)
        {
            return new Vector3(color.R, color.G, color.B) / 255f;
        }

        // Jedna składowa koloru: przycięcie do zakresu 0-1 i przeliczenie na 0-255.
        private static int ToByte(float value)
        {
            return (int)MathF.Round(Math.Clamp(value, 0f, 1f) * 255);
        }
    }
}