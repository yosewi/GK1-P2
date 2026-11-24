using System.Numerics;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.Lighting
{
    // Zamknięta krzywa Béziera w płaszczyźnie xy, po której porusza się źródło światła.
    public sealed class LightPath
    {
        public List<Vector2> ControlPoints { get; } = new List<Vector2>
        {
            new Vector2(220, 0),
            new Vector2(220, 350),
            new Vector2(-350, 350),
            new Vector2(-350, 0),
            new Vector2(-350, -350),
            new Vector2(220, -350),
        };

        // Punkt krzywej dla parametru t od 0 do 1
        public Vector2 PointAt(float t)
        {
            float wrapped = t - MathF.Floor(t);

            int degree = ControlPoints.Count;

            Vector2 result = Vector2.Zero;
            for (int i = 0; i <= degree; i++)
            {
                Vector2 controlPoint = ControlPoints[i % ControlPoints.Count];
                result += controlPoint * Bernstein.Polynomial(i, degree, wrapped);
            }

            return result;
        }
    }
}
