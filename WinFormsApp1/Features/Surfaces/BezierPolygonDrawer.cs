using System.Numerics;

namespace WinFormsApp1.Features.Surfaces
{
    /// <summary>
    /// Rysuje wielobok Béziera (siatkę kontrolną) jednej powierzchni:
    ///  - 16 punktów kontrolnych jako małe kółka,
    ///  - linie łączące sąsiednie punkty w każdym wierszu i w każdej kolumnie siatki 4 x 4.
    ///
    /// Rysujemy punkty PO obrocie sceny i robimy rzut prostokątny na płaszczyznę xy:
    /// bierzemy tylko x i y obróconego punktu, a z pomijamy.
    /// Graphics musi mieć już ustawioną transformację do współrzędnych świata (ScreenMapper).
    /// </summary>
    public static class BezierPolygonDrawer
    {
        private const float PointRadius = 4f;

        public static void Draw(Graphics g, BezierSurface surface, Color color)
        {
            Vector3[,] points = surface.RotatedControlPoints;
            int n = BezierSurface.PointsPerSide;

            using Pen linePen = new Pen(color, 1f);
            using Brush pointBrush = new SolidBrush(color);

            // Linie w wierszach: V[i, j] -> V[i, j + 1]
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    DrawSegment(g, linePen, points[i, j], points[i, j + 1]);
                }
            }

            // Linie w kolumnach: V[i, j] -> V[i + 1, j]
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    DrawSegment(g, linePen, points[i, j], points[i + 1, j]);
                }
            }

            // Punkty kontrolne rysujemy na końcu, żeby były na wierzchu linii.
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
            // Rzut prostokątny: z pomijamy.
            g.DrawLine(pen, from.X, from.Y, to.X, to.Y);
        }

        private static void DrawPoint(Graphics g, Brush brush, Vector3 point)
        {
            g.FillEllipse(brush, point.X - PointRadius, point.Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
        }
    }
}
