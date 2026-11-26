using System.Numerics;

namespace BezierSurfaces.Features.Lighting
{
    // Źródło światła.
    // Porusza się po zamkniętej krzywej Béziera leżącej na wysokości z.
    public sealed class LightSource
    {
        public const int DefaultHeight = 500;
        public const int MinLightCount = 1;
        public const int MaxLightCount = 4;
        public const int DefaultReflectorM = 20;

        public LightPath Path { get; } = new LightPath();

        // Miejsce pierwszego światła na krzywej jako ułamek jej długości: od 0 do 1 to jedno pełne okrążenie.
        public float PathParameter { get; private set; }

        // Wysokość z płaszczyzny, w której leży krzywa.
        public float Height { get; private set; } = DefaultHeight;

        public int LightCount { get; private set; } = MinLightCount;

        public LightType Type { get; set; } = LightType.Point;

        public int ReflectorM { get; set; } = DefaultReflectorM;

        public ActiveLights GetActiveLights()
        {
            List<Vector3> positions = Type == LightType.Directional ? new List<Vector3>() : Positions();
            return new ActiveLights(Type, positions, ReflectorM);
        }

        // Położenia wszystkich świateł
        public List<Vector3> Positions()
        {
            return Path.EvenlySpacedPoints(PathParameter, LightCount)
                .Select(point => new Vector3(point.X, point.Y, Height))
                .ToList();
        }

        public void SetLightCount(int count)
        {
            LightCount = Math.Clamp(count, MinLightCount, MaxLightCount);
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