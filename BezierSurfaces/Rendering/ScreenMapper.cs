using System.Numerics;

namespace BezierSurfaces.Rendering
{
    // Zamienia punkt sceny na piksel ekranu.
    public sealed class ScreenMapper
    {
        public float CenterX { get; }
        public float CenterY { get; }

        public ScreenMapper(int width, int height)
        {
            CenterX = width / 2f;
            CenterY = height / 2f;
        }

        public Vector2 ToScreen(Vector3 worldPoint)
        {
            return new Vector2(CenterX + worldPoint.X, CenterY - worldPoint.Y);
        }

        public Vector3 ToWorld(float screenX, float screenY, float z)
        {
            return new Vector3(screenX - CenterX, CenterY - screenY, z);
        }

        // To samo przejście ze sceny na ekran, ale dla linii rysowanych przez Graphics:
        public void ApplyWorldTransform(Graphics g)
        {
            g.ScaleTransform(1, -1);
            g.TranslateTransform(CenterX, -CenterY);
        }
    }
}
