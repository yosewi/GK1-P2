using System.Numerics;

namespace BezierSurfaces.Rendering
{
    // Zamienia punkt sceny na piksel ekranu, pomijając głębokość z.
    public sealed class ScreenMapper
    {
        // Środek obszaru rysowania w pikselach.
        public float CenterX { get; }
        public float CenterY { get; }

        public ScreenMapper(int width, int height)
        {
            CenterX = width / 2f;
            CenterY = height / 2f;
        }

        // Pomija z. Oś y odwracamy, bo na ekranie y rośnie w dół, a w scenie w górę.
        public Vector2 ToScreen(Vector3 worldPoint)
        {
            return new Vector2(CenterX + worldPoint.X, CenterY - worldPoint.Y);
        }

        // To samo przejście ze sceny na ekran, ale dla linii rysowanych przez Graphics:
        public void ApplyWorldTransform(Graphics g)
        {
            g.ScaleTransform(1, -1);
            g.TranslateTransform(CenterX, -CenterY);
        }
    }
}
