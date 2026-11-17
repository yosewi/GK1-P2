using System.Numerics;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.Lighting
{
    // Zamknięta krzywa Béziera w płaszczyźnie xy, po której porusza się źródło światła.
    // Składa się z kilku odcinków Béziera stopnia 3 połączonych w pętlę: ostatni odcinek kończy się
    // w punkcie, w którym zaczyna się pierwszy.
    public sealed class LightPath
    {
        private const int Degree = 3;
        private const int DefaultSegmentCount = 4;
        private const float DefaultRadius = 250f;

        // Długość "uchwytów" przy punktach na okręgu, dla której cztery odcinki dają prawie idealny okrąg.
        private const float CircleHandleFactor = 0.5523f;

        // Punkty kontrolne kolejnych odcinków.
        public List<Vector2> ControlPoints { get; } = new List<Vector2>();

        // Na starcie krzywa jest okręgiem wokół środka sceny.
        public LightPath()
        {
            float handle = CircleHandleFactor * DefaultRadius;

            for (int segment = 0; segment < DefaultSegmentCount; segment++)
            {
                Vector2 start = PointOnCircle(segment);
                Vector2 end = PointOnCircle(segment + 1);

                ControlPoints.Add(start);
                ControlPoints.Add(start + handle * TangentOnCircle(segment));
                ControlPoints.Add(end - handle * TangentOnCircle(segment + 1));
            }
        }

        // Punkt krzywej dla parametru t od 0 do 1 (t = 1 to znowu punkt startowy).
        // Najpierw wybieramy odcinek, potem liczymy punkt na nim tak samo jak dla powierzchni Béziera.
        public Vector2 PointAt(float t)
        {
            int segmentCount = ControlPoints.Count / Degree;

            float wrapped = t - MathF.Floor(t);
            float scaled = wrapped * segmentCount;
            int segment = Math.Min((int)scaled, segmentCount - 1);
            float local = scaled - segment;

            Vector2 result = Vector2.Zero;
            for (int i = 0; i <= Degree; i++)
            {
                Vector2 controlPoint = ControlPoints[(segment * Degree + i) % ControlPoints.Count];
                result += controlPoint * Bernstein.Polynomial(i, Degree, local);
            }

            return result;
        }

        // Punkt startowy odcinka numer index - kolejne punkty co ćwierć okręgu.
        private static Vector2 PointOnCircle(int index)
        {
            float angle = index * MathF.PI / 2;
            return DefaultRadius * new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        // Kierunek styczny do okręgu w tym punkcie (prostopadły do promienia).
        private static Vector2 TangentOnCircle(int index)
        {
            float angle = index * MathF.PI / 2;
            return new Vector2(-MathF.Sin(angle), MathF.Cos(angle));
        }
    }
}
