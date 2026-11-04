using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WinFormsApp1.Rendering
{
    /// <summary>
    /// Szybka bitmapa: piksele trzymamy w zwykłej tablicy int[],
    /// a obiekt Bitmap korzysta bezpośrednio z tej samej pamięci.
    ///
    /// Dzięki temu zapis piksela to jedno przypisanie do tablicy
    /// (bez blokowania bitmapy jak w Bitmap.SetPixel), a mimo to
    /// możemy wyświetlić wynik w PictureBox i rysować po nim przez Graphics.
    /// </summary>
    public sealed class DirectBitmap : IDisposable
    {
        public int Width { get; }
        public int Height { get; }

        /// <summary>
        /// Piksele w kolejności wierszami: piksel (x, y) ma indeks y * Width + x.
        /// Każdy int to kolor w formacie ARGB (np. Color.Red.ToArgb()).
        /// </summary>
        public int[] Pixels { get; }

        /// <summary>
        /// Zwykła bitmapa .NET, która "patrzy" na tablicę Pixels.
        /// Ją podajemy do PictureBox i do Graphics.FromImage.
        /// </summary>
        public Bitmap Bitmap { get; }

        // Uchwyt, który przypina tablicę w pamięci.
        // Bez tego garbage collector mógłby przesunąć tablicę,
        // a Bitmap pisałaby/czytała spod starego adresu.
        private GCHandle _pixelsHandle;
        private bool _disposed;

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;

            Pixels = new int[width * height];
            _pixelsHandle = GCHandle.Alloc(Pixels, GCHandleType.Pinned);

            IntPtr pixelsAddress = _pixelsHandle.AddrOfPinnedObject();
            int bytesPerRow = width * 4; // 4 bajty na piksel (A, R, G, B)

            Bitmap = new Bitmap(width, height, bytesPerRow, PixelFormat.Format32bppPArgb, pixelsAddress);
        }

        /// <summary>
        /// Ustawia kolor jednego piksela. Piksele poza bitmapą są pomijane.
        /// (x, y) to współrzędne PIKSELA: (0, 0) w lewym górnym rogu, y w dół.
        /// </summary>
        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return;
            }

            Pixels[y * Width + x] = color.ToArgb();
        }

        /// <summary>
        /// Zamalowuje całą bitmapę jednym kolorem.
        /// </summary>
        public void Clear(Color color)
        {
            Array.Fill(Pixels, color.ToArgb());
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Bitmap.Dispose();
            _pixelsHandle.Free();
        }
    }
}
