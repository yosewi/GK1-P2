using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace BezierSurfaces.Rendering
{
    public sealed class DirectBitmap : IDisposable
    {
        public int Width { get; }
        public int Height { get; }

        public int[] Pixels { get; }

        public Bitmap Bitmap { get; }

        private GCHandle _pixelsHandle;
        private bool _disposed;

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;

            Pixels = new int[width * height];
            _pixelsHandle = GCHandle.Alloc(Pixels, GCHandleType.Pinned);

            IntPtr pixelsAddress = _pixelsHandle.AddrOfPinnedObject();
            int bytesPerRow = width * 4;

            Bitmap = new Bitmap(width, height, bytesPerRow, PixelFormat.Format32bppPArgb, pixelsAddress);
        }

        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return;
            }

            Pixels[y * Width + x] = color.ToArgb();
        }

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
