using System.Numerics;

namespace BezierSurfaces.Features.Lighting
{
    // Rysuje krzywą, po której porusza się światło, jej punkty kontrolne i bieżące położenie światła.
    public static class LightPathDrawer
    {
        // Z ilu odcinków prostych składamy rysowaną krzywą.
        private const int CurveSamples = 200;

        private const float ControlPointRadius = 4f;
        private const float LightRadius = 8f;

        private static readonly Color CurveColor = Color.Goldenrod;
        private static readonly Color ControlPolygonColor = Color.FromArgb(140, Color.Gray);
        private static readonly Color LightFillColor = Color.Gold;
        private static readonly Color LightOutlineColor = Color.Black;

        // Punkty kontrolne rysujemy tylko wtedy, gdy showControlPoints jest włączone.
        public static void Draw(Graphics g, LightSource light, bool showControlPoints)
        {
            DrawCurve(g, light.Path);

            if (showControlPoints)
            {
                DrawControlPoints(g, light.Path);
            }

            DrawLight(g, light.Position);
        }

        // Krzywa jako łamana z wielu krótkich odcinków. Ostatni punkt to znowu pierwszy, więc łamana jest zamknięta.
        private static void DrawCurve(Graphics g, LightPath path)
        {
            PointF[] points = new PointF[CurveSamples + 1];
            for (int i = 0; i <= CurveSamples; i++)
            {
                Vector2 point = path.PointAt((float)i / CurveSamples);
                points[i] = new PointF(point.X, point.Y);
            }

            using Pen pen = new Pen(CurveColor, 2f);
            g.DrawLines(pen, points);
        }

        // Wielobok kontrolny (punkty połączone po kolei w pętlę) i kółka w punktach kontrolnych.
        private static void DrawControlPoints(Graphics g, LightPath path)
        {
            PointF[] points = path.ControlPoints.Select(point => new PointF(point.X, point.Y)).ToArray();

            using Pen pen = new Pen(ControlPolygonColor, 1f);
            g.DrawPolygon(pen, points);

            using Brush brush = new SolidBrush(CurveColor);
            foreach (PointF point in points)
            {
                g.FillEllipse(brush, point.X - ControlPointRadius, point.Y - ControlPointRadius,
                    2 * ControlPointRadius, 2 * ControlPointRadius);
            }
        }

        // Światło jako żółte kółko z czarną obwódką.
        private static void DrawLight(Graphics g, Vector3 position)
        {
            float left = position.X - LightRadius;
            float top = position.Y - LightRadius;
            float size = 2 * LightRadius;

            using Brush brush = new SolidBrush(LightFillColor);
            g.FillEllipse(brush, left, top, size, size);

            using Pen pen = new Pen(LightOutlineColor, 1.5f);
            g.DrawEllipse(pen, left, top, size, size);
        }
    }
}
