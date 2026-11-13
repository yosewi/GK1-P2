using System.Numerics;
using FastBitmapLib;

namespace BezierSurfaces.Features.Texturing
{
    // Obrazek, z którego odczytujemy kolor obiektu dla punktu powierzchni o parametrach u, v.
    // Kolory trzymamy od razu jako wektory (R, G, B) z zakresu 0-1, tak jak w modelu oświetlenia.
    public sealed class Texture
    {
        private readonly Vector3[] _colors;

        // Rozmiar obrazka w pikselach.
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

        // Wczytuje teksturę z pliku graficznego
        public static Texture Load(string path)
        {
            using Bitmap image = new Bitmap(path);
            return new Texture(image);
        }

        // Kolor dla parametrów u, v z przedziału od 0 do 1: u wybiera kolumnę obrazka, v wiersz.
        // Bierzemy najbliższy piksel.
        public Vector3 Sample(float u, float v)
        {
            int x = Math.Clamp((int)MathF.Round(u * (Width - 1)), 0, Width - 1);
            int y = Math.Clamp((int)MathF.Round(v * (Height - 1)), 0, Height - 1);
            return _colors[y * Width + x];
        }
    }
}
