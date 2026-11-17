using System.Numerics;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.Lighting
{
    // Zamknięta krzywa Béziera w płaszczyźnie xy, po której porusza się źródło światła.
    // To jedna krzywa Béziera, której ostatni punkt kontrolny jest tym samym punktem co pierwszy
    public sealed class LightPath
    {
        // Różne punkty kontrolne V0, V1, ..., V5. Ostatni punkt krzywej (V6) to zawsze V0, więc go nie przechowujemy.
        public List<Vector2> ControlPoints { get; } = new List<Vector2>
        {
            new Vector2(220, 0),
            new Vector2(220, 350),
            new Vector2(-350, 350),
            new Vector2(-350, 0),
            new Vector2(-350, -350),
            new Vector2(220, -350),
        };

        // Punkt krzywej dla parametru t od 0 do 1 (t = 1 to znowu punkt startowy).
        // Ten sam wzór co dla powierzchni Béziera, tylko w jednym kierunku: suma punktów razy wielomiany Bernsteina.
        public Vector2 PointAt(float t)
        {
            float wrapped = t - MathF.Floor(t);

            int degree = ControlPoints.Count;

            Vector2 result = Vector2.Zero;
            for (int i = 0; i <= degree; i++)
            {
                // Dla i równego stopniowi bierzemy znowu V0
                Vector2 controlPoint = ControlPoints[i % ControlPoints.Count];
                result += controlPoint * Bernstein.Polynomial(i, degree, wrapped);
            }

            return result;
        }
    }
}
