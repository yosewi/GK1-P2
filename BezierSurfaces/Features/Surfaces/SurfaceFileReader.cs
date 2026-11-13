using System.Globalization;
using System.Numerics;

namespace BezierSurfaces.Features.Surfaces
{
    // Wczytuje powierzchnie z pliku tekstowego: w każdej linii jeden punkt (x y z), po 16 punktów na powierzchnię.
    public static class SurfaceFileReader
    {
        // Zwraca wszystkie powierzchnie z pliku
        public static List<BezierSurface> Read(string path)
        {
            List<Vector3> points = ReadAllPoints(path);
            List<BezierSurface> surfaces = new List<BezierSurface>();

            // Dzielimy listę punktów na kawałki po 16. Każdy kawałek to jedna powierzchnia.
            for (int start = 0; start < points.Count; start += BezierSurface.ControlPointCount)
            {
                List<Vector3> pointsOfOneSurface = points.GetRange(start, BezierSurface.ControlPointCount);
                surfaces.Add(new BezierSurface(pointsOfOneSurface));
            }

            return surfaces;
        }

        // Czyta wszystkie linie pliku, pomija puste, a pozostałe zamienia na punkty.
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

        // Zamienia jedną linię na punkt
        private static Vector3 ParsePoint(string line, int lineNumber)
        {
            string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            float x = ParseNumber(parts[0], lineNumber);
            float y = ParseNumber(parts[1], lineNumber);
            float z = ParseNumber(parts[2], lineNumber);

            return new Vector3(x, y, z);
        }

        // Zamienia tekst na liczbę
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
