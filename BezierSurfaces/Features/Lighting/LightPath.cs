using System.Numerics;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.Lighting
{
    // Zamknięta krzywa Béziera w płaszczyźnie xy, po której porusza się źródło światła.
    public sealed class LightPath
    {
        // Z ilu odcinków składamy łamaną do mierzenia długości krzywej.
        private const int LengthSamples = 512;

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

        public List<Vector2> EvenlySpacedPoints(float startFraction, int count)
        {
            // cumulative[i] = długość łamanej od t = 0 do t = i / LengthSamples.
            float[] cumulative = new float[LengthSamples + 1];
            Vector2 previous = PointAt(0);
            for (int i = 1; i <= LengthSamples; i++)
            {
                Vector2 current = PointAt((float)i / LengthSamples);
                cumulative[i] = cumulative[i - 1] + Vector2.Distance(previous, current);
                previous = current;
            }

            float totalLength = cumulative[LengthSamples];
            List<Vector2> points = new List<Vector2>(count);

            for (int k = 0; k < count; k++)
            {
                float fraction = startFraction + (float)k / count;
                fraction -= MathF.Floor(fraction);
                points.Add(PointAt(ParameterAtLength(cumulative, fraction * totalLength)));
            }

            return points;
        }

        // Parametr t, dla którego łamana od początku krzywej ma długość length.
        private static float ParameterAtLength(float[] cumulative, float length)
        {
            float totalLength = cumulative[LengthSamples];
            if (totalLength <= 0)
            {
                return 0;
            }

            int low = 0;
            int high = LengthSamples;
            while (high - low > 1)
            {
                int middle = (low + high) / 2;
                if (cumulative[middle] <= length)
                {
                    low = middle;
                }
                else
                {
                    high = middle;
                }
            }

            float segmentLength = cumulative[high] - cumulative[low];
            float local = segmentLength > 0 ? (length - cumulative[low]) / segmentLength : 0;
            return (low + local) / LengthSamples;
        }
    }
}
