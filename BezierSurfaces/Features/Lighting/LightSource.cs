using System.Numerics;

namespace BezierSurfaces.Features.Lighting
{
    // Źródło światła. Stoi w układzie sceny, więc nie obraca się razem z powierzchniami.
    public sealed class LightSource
    {
        // Wysokość startowa.
        public const int DefaultHeight = 500;

        // Położenie światła. Na starcie nad środkiem sceny.
        public Vector3 Position { get; private set; } = new Vector3(0, 0, DefaultHeight);
    }
}