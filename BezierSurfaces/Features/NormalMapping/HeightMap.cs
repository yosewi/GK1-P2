using System.Numerics;
using BezierSurfaces.Features.Texturing;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.NormalMapping
{
    // Mapa wysokości
    // Do wektora normalnego dodajemy wektor D, prostopadły do N:
    // D = k * (Hu * (N x Pv) - Hv * (N x Pu)),   N' = normalize(N + D)
    // Hu = (H[u+1] - H[u-1]) / 2,   Hv = (H[v+1] - H[v-1]) / 2
    public sealed class HeightMap
    {
        public const int DefaultK = 20;

        private readonly Texture _image;

        // Hu i Hv w każdym pikselu obrazka
        private readonly float[] _hu;
        private readonly float[] _hv;

        public float K { get; set; } = DefaultK;

        public HeightMap(Texture image)
        {
            _image = image;
            int width = image.Width;
            int height = image.Height;

            _hu = new float[width * height];
            _hv = new float[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float left = Gray(Math.Max(x - 1, 0), y);
                    float right = Gray(Math.Min(x + 1, width - 1), y);
                    float above = Gray(x, Math.Max(y - 1, 0));
                    float below = Gray(x, Math.Min(y + 1, height - 1));

                    _hu[y * width + x] = (right - left) / 2f;
                    _hv[y * width + x] = (above - below) / 2f;
                }
            }
        }

        public static HeightMap Load(string path)
        {
            return new HeightMap(Texture.Load(path));
        }

        // Zmienia wektor normalny powierzchni na podstawie mapy wysokości w miejscu (u, v).
        public Vector3 Apply(Vector3 surfaceNormal, Vector3 tangentU, Vector3 tangentV, float u, float v)
        {
            int index = _image.PixelRow(v) * _image.Width + _image.PixelColumn(u);
            float hu = _hu[index];
            float hv = _hv[index];

            Vector3 pu = VectorMath.SafeNormalize(tangentU);
            Vector3 pv = VectorMath.SafeNormalize(tangentV);

            Vector3 d = K * (hu * Vector3.Cross(surfaceNormal, pv) - hv * Vector3.Cross(surfaceNormal, pu));
            return VectorMath.SafeNormalize(surfaceNormal + d);
        }

        // Wysokość piksela: średnia z R, G, B, od 0 do 1.
        private float Gray(int x, int y)
        {
            Vector3 color = _image.GetPixel(x, y);
            return (color.X + color.Y + color.Z) / 3f;
        }
    }
}
