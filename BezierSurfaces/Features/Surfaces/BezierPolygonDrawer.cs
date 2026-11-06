using System.Numerics;

namespace BezierSurfaces.Features.Surfaces
{
    public static class BezierPolygonDrawer
    {
        private const float PointRadius = 4f;

        public static void Draw(Graphics g, BezierSurface surface, Color color)
        {
            Vector3[,] points = surface.RotatedControlPoints;
            int n = BezierSurface.PointsPerSide;

            using Pen linePen = new Pen(color, 1f);
            using Brush pointBrush = new SolidBrush(color);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    DrawSegment(g, linePen, points[i, j], points[i, j + 1]);
                }
            }

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    DrawSegment(g, linePen, points[i, j], points[i + 1, j]);
                }
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    DrawPoint(g, pointBrush, points[i, j]);
                }
            }
        }

        private static void DrawSegment(Graphics g, Pen pen, Vector3 from, Vector3 to)
        {
            g.DrawLine(pen, from.X, from.Y, to.X, to.Y);
        }

        private static void DrawPoint(Graphics g, Brush brush, Vector3 point)
        {
            g.FillEllipse(brush, point.X - PointRadius, point.Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
        }
    }
}
