using System.Numerics;

namespace BezierSurfaces.Features.Lighting
{
    // Źródło światła. Stoi w układzie sceny, więc nie obraca się razem z powierzchniami.
    // Porusza się po zamkniętej krzywej Béziera leżącej na wysokości z.
    public sealed class LightSource
    {
        // Wysokość startowa.
        public const int DefaultHeight = 500;

        // Krzywa, po której porusza się światło.
        public LightPath Path { get; } = new LightPath();

        // Miejsce na krzywej: wartości od 0 do 1 to jedno pełne okrążenie.
        public float PathParameter { get; private set; }

        // Wysokość z płaszczyzny, w której leży krzywa.
        public float Height { get; private set; } = DefaultHeight;

        // Położenie światła: bieżący punkt krzywej na wysokości z.
        public Vector3 Position
        {
            get
            {
                Vector2 point = Path.PointAt(PathParameter);
                return new Vector3(point.X, point.Y, Height);
            }
        }

        // Zmienia wysokość źródła światła.
        public void SetHeight(float z)
        {
            Height = z;
        }

        // Przesuwa światło dalej po krzywej. Po pełnym okrążeniu zaczyna od początku.
        public void Advance(float step)
        {
            PathParameter = (PathParameter + step) % 1f;
        }
    }
}