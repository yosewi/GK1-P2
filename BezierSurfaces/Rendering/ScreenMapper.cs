namespace BezierSurfaces.Rendering
{
    public sealed class ScreenMapper
    {
        public float CenterX { get; }
        public float CenterY { get; }

        public ScreenMapper(int width, int height)
        {
            CenterX = width / 2f;
            CenterY = height / 2f;
        }

        public Point ToPixel(float x, float y)
        {
            int px = (int)MathF.Round(CenterX + x);
            int py = (int)MathF.Round(CenterY - y);
            return new Point(px, py);
        }

        public void ApplyWorldTransform(Graphics g)
        {
            g.ScaleTransform(1, -1);
            g.TranslateTransform(CenterX, -CenterY);
        }
    }
}
