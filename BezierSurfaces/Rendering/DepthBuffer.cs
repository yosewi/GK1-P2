namespace BezierSurfaces.Rendering
{
    public sealed class DepthBuffer
    {
        public int Width { get; }
        public int Height { get; }

        private readonly float[] _depths;

        public DepthBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            _depths = new float[width * height];
            Clear();
        }

        public void Clear()
        {
            Array.Fill(_depths, float.NegativeInfinity);
        }

        public bool TryUpdate(int x, int y, float z)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false;
            }

            int index = y * Width + x;
            bool isCloserToViewer = z > _depths[index];

            if (isCloserToViewer)
            {
                _depths[index] = z;
            }

            return isCloserToViewer;
        }
    }
}
