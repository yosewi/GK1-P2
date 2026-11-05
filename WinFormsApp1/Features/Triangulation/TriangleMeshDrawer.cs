using System.Numerics;

namespace WinFormsApp1.Features.Triangulation
{
    /// <summary>
    /// Rysuje siatkę trójkątów: krawędzie każdego trójkąta.
    ///
    /// Bierzemy wierzchołki PO obrocie i robimy rzut prostokątny na płaszczyznę xy (x, y, bez z).
    /// Graphics musi mieć już ustawioną transformację do współrzędnych świata (ScreenMapper).
    /// Wspólne krawędzie sąsiednich trójkątów rysują się dwa razy - to nic nie zmienia w obrazie.
    /// </summary>
    public static class TriangleMeshDrawer
    {
        public static void Draw(Graphics g, TriangleMesh mesh, Color color)
        {
            using Pen pen = new Pen(color, 1f);

            foreach (Triangle triangle in mesh.Triangles)
            {
                DrawEdge(g, pen, triangle.A, triangle.B);
                DrawEdge(g, pen, triangle.B, triangle.C);
                DrawEdge(g, pen, triangle.C, triangle.A);
            }
        }

        private static void DrawEdge(Graphics g, Pen pen, Vertex from, Vertex to)
        {
            Vector3 a = from.RotatedPosition;
            Vector3 b = to.RotatedPosition;
            g.DrawLine(pen, a.X, a.Y, b.X, b.Y); // rzut prostokątny: z pomijamy
        }
    }
}
