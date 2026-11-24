using System.Numerics;

namespace BezierSurfaces.Features.Lighting
{
    // Źródło światła.
    // Porusza się po zamkniętej krzywej Béziera leżącej na wysokości z.
    public sealed class LightSource
    {
        public const int DefaultHeight = 500;

        public LightPath Path { get; } = new LightPath();

        // Miejsce na krzywej: wartości od 0 do 1 to jedno pełne okrążenie.
        public float PathParameter { get; private set; }

        // Wysokość z płaszczyzny, w której leży krzywa.
        public float Height { get; private set; } = DefaultHeight;

        public Vector3 Position
        {
            get
            {
                Vector2 point = Path.PointAt(PathParameter);
                return new Vector3(point.X, point.Y, Height);
            }
        }

        public void SetHeight(float z)
        {
            Height = z;
        }

        public void Advance(float step)
        {
            PathParameter = (PathParameter + step) % 1f;
        }
    }
}