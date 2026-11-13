namespace BezierSurfaces.Rendering
{
    // Z-bufor: dla każdego piksela pamięta głębokość najbliższego narysowanego dotąd punktu.
    public sealed class DepthBuffer
    {
        public int Width { get; }
        public int Height { get; }

        // Głębokości wszystkich pikseli.
        private readonly float[] _depths;

        public DepthBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            _depths = new float[width * height];
            Clear();
        }

        // Początek - każdy piksel jest nieskończenie daleko.
        public void Clear()
        {
            Array.Fill(_depths, float.NegativeInfinity);
        }

        // Sprawdza, czy punkt o głębokości z jest bliżej niż to, co już jest w pikselu.
        // Jeśli tak, zapamiętuje nowe z i zwraca true, czyli piksel trzeba narysować.
        public bool TryUpdate(int x, int y, float z)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false;
            }

            int index = y * Width + x;

            // Większe z jest bliżej 
            bool isCloserToViewer = z > _depths[index];

            if (isCloserToViewer)
            {
                _depths[index] = z;
            }

            return isCloserToViewer;
        }
    }
}
