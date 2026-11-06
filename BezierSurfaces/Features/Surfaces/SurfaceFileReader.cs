using System.Globalization;
using System.Numerics;

namespace BezierSurfaces.Features.Surfaces
{
    public static class SurfaceFileReader
    {
        public static List<BezierSurface> Read(string path)
        {
            List<Vector3> points = ReadAllPoints(path);

            if (points.Count == 0 || points.Count % BezierSurface.ControlPointCount != 0)
            {
                throw new FormatException(
                    $"The file must contain a multiple of {BezierSurface.ControlPointCount} points " +
                    $"(16 per surface), but it contains {points.Count}.");
            }

            List<BezierSurface> surfaces = new List<BezierSurface>();

            for (int start = 0; start < points.Count; start += BezierSurface.ControlPointCount)
            {
                List<Vector3> pointsOfOneSurface = points.GetRange(start, BezierSurface.ControlPointCount);
                surfaces.Add(new BezierSurface(pointsOfOneSurface));
            }

            return surfaces;
        }

        private static List<Vector3> ReadAllPoints(string path)
        {
            string[] lines = File.ReadAllLines(path);
            List<Vector3> points = new List<Vector3>();

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex].Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                int lineNumber = lineIndex + 1;
                points.Add(ParsePoint(line, lineNumber));
            }

            return points;
        }

        private static Vector3 ParsePoint(string line, int lineNumber)
        {
            string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
            {
                throw new FormatException($"Line {lineNumber}: expected 3 numbers (x y z), found {parts.Length}.");
            }

            float x = ParseNumber(parts[0], lineNumber);
            float y = ParseNumber(parts[1], lineNumber);
            float z = ParseNumber(parts[2], lineNumber);

            return new Vector3(x, y, z);
        }

        private static float ParseNumber(string text, int lineNumber)
        {
            if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            {
                throw new FormatException($"Line {lineNumber}: \"{text}\" is not a number.");
            }

            return value;
        }
    }
}
