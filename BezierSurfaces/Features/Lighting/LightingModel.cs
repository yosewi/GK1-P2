using System.Numerics;

namespace BezierSurfaces.Features.Lighting
{
    // Model oświetlenia: składowa rozproszona (Lambert) i składowa zwierciadlana.
    // Kolory trzymamy jako wektory (R, G, B) z wartościami od 0 do 1.
    public sealed class LightingModel
    {
        // Wartości startowe z suwaków.
        public const float DefaultKd = 0.5f;
        public const float DefaultKs = 0.5f;
        public const int DefaultM = 20;

        // Kierunek do obserwatora wzdłuż osi z.
        private static readonly Vector3 ViewerDirection = Vector3.UnitZ;

        // Wpływ składowej rozproszonej, od 0 do 1.
        public float Kd { get; set; } = DefaultKd;

        // Wpływ składowej zwierciadlanej, od 0 do 1.
        public float Ks { get; set; } = DefaultKs;

        // Jak bardzo powierzchnia błyszczy: im większe, tym mniejszy i ostrzejszy odblask.
        public int M { get; set; } = DefaultM;

        // Kolor światła.
        public Vector3 LightColor { get; set; } = Vector3.One;

        // Kolor punktu powierzchni o danym wektorze normalnym, oświetlonego z danego kierunku.
        public Vector3 Compute(Vector3 normal, Vector3 toLight, Vector3 objectColor)
        {
            float normalDotLight = Vector3.Dot(normal, toLight);

            // Składowa rozproszona: im bardziej powierzchnia jest zwrócona do światła, tym jaśniej.
            // Ujemny cosinus oznacza światło z tyłu powierzchni, więc przyjmujemy zero.
            float diffuse = MathF.Max(0, normalDotLight);

            // Kierunek, w którym światło odbija się od powierzchni.
            Vector3 reflected = 2 * normalDotLight * normal - toLight;

            // Składowa zwierciadlana: odblask, gdy odbite światło leci prosto do obserwatora.
            float specular = MathF.Pow(MathF.Max(0, Vector3.Dot(ViewerDirection, reflected)), M);

            // liczenie kolorów.
            Vector3 lightTimesObject = LightColor * objectColor;
            return Kd * diffuse * lightTimesObject + Ks * specular * lightTimesObject;
        }

        // Zamienia kolor z zakresu 0-1 na Color z zakresu 0-255.
        public static Color ToColor(Vector3 color)
        {
            return Color.FromArgb(ToByte(color.X), ToByte(color.Y), ToByte(color.Z));
        }

        // Zamienia Color z zakresu 0-255 na kolor z zakresu 0-1.
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