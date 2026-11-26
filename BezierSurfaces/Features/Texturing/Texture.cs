using System.Numerics;
using FastBitmapLib;

namespace BezierSurfaces.Features.Texturing
{
    // Obrazek, z którego odczytujemy kolor obiektu dla punktu powierzchni o parametrach u, v.
    public sealed class Texture
    {
        private readonly Vector3[] _colors;

        public int Width { get; }
        public int Height { get; }

        // Raz przepisuje wszystkie piksele obrazka do tablicy kolorów.
        public Texture(Bitmap image)
        {
            Width = image.Width;
            Height = image.Height;
            _colors = new Vector3[Width * Height];

            using Bitmap copy = new Bitmap(image);
            using FastBitmap pixels = copy.FastLock();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Color color = pixels.GetPixel(x, y);
                    _colors[y * Width + x] = new Vector3(color.R, color.G, color.B) / 255f;
                }
            }
        }

        public static Texture Load(string path)
        {
            using Bitmap image = new Bitmap(path);
            return new Texture(image);
        }

        // Kolor dla parametrów u, v z przedziału od 0 do 1: u wybiera kolumnę obrazka, v wiersz.
        public Vector3 Sample(float u, float v)
        {
            return GetPixel(PixelColumn(u), PixelRow(v));
        }

        // Kolumna obrazka dla parametru u.
        public int PixelColumn(float u) => Math.Clamp((int)MathF.Round(u * (Width - 1)), 0, Width - 1);

        // Wiersz obrazka dla parametru v
        public int PixelRow(float v) => Math.Clamp((int)MathF.Round((1 - v) * (Height - 1)), 0, Height - 1);

        // Kolor piksela obrazka (x, y).
        public Vector3 GetPixel(int x, int y) => _colors[y * Width + x];
    }
}
