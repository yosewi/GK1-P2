using System.Numerics;
using WinFormsApp1.Features.Surfaces;

namespace WinFormsApp1.Features.Triangulation
{
    /// <summary>
    /// Triangulacja powierzchni Béziera: zamiana powierzchni na siatkę trójkątów.
    ///
    /// 1. Dzielimy przedziały u ∈ [0, 1] i v ∈ [0, 1] na "steps" równych odcinków.
    ///    Dla każdej pary (u, v) = (i / steps, j / steps) liczymy wierzchołek: P, Pu, Pv (i N).
    ///    Wychodzi siatka (steps + 1) x (steps + 1) wierzchołków.
    ///
    /// 2. Każdy kwadracik siatki dzielimy przekątną na 2 trójkąty:
    ///
    ///        v ↑
    ///   [i, j+1] ●───────● [i+1, j+1]
    ///            │ ╲  2  │
    ///            │  ╲    │
    ///            │ 1 ╲   │
    ///     [i, j] ●───────● [i+1, j]   → u
    ///
    ///    trójkąt 1: [i, j], [i+1, j], [i, j+1]
    ///    trójkąt 2: [i+1, j], [i+1, j+1], [i, j+1]
    /// </summary>
    public static class MeshBuilder
    {
        public const int MinSteps = 1;

        public static TriangleMesh Build(BezierSurface surface, int steps)
        {
            if (steps < MinSteps)
            {
                throw new ArgumentOutOfRangeException(nameof(steps), $"Steps must be at least {MinSteps}.");
            }

            // Tablica wierzchołków jest tylko pomocnicza, na czas budowania: dzięki niej sąsiednie
            // trójkąty dostają TEN SAM obiekt Vertex. Sama siatka przechowuje tylko listę trójkątów.
            Vertex[,] vertices = BuildVertices(surface, steps);
            List<Triangle> triangles = BuildTriangles(vertices, steps);

            return new TriangleMesh(triangles);
        }

        private static Vertex[,] BuildVertices(BezierSurface surface, int steps)
        {
            int count = steps + 1;
            Vertex[,] vertices = new Vertex[count, count];

            for (int i = 0; i < count; i++)
            {
                float u = (float)i / steps;

                for (int j = 0; j < count; j++)
                {
                    float v = (float)j / steps;

                    Vector3 position = surface.Point(u, v);
                    Vector3 tangentU = surface.TangentU(u, v);
                    Vector3 tangentV = surface.TangentV(u, v);

                    vertices[i, j] = new Vertex(u, v, position, tangentU, tangentV);
                }
            }

            return vertices;
        }

        private static List<Triangle> BuildTriangles(Vertex[,] vertices, int steps)
        {
            List<Triangle> triangles = new List<Triangle>(2 * steps * steps);

            for (int i = 0; i < steps; i++)
            {
                for (int j = 0; j < steps; j++)
                {
                    Vertex bottomLeft = vertices[i, j];
                    Vertex bottomRight = vertices[i + 1, j];
                    Vertex topLeft = vertices[i, j + 1];
                    Vertex topRight = vertices[i + 1, j + 1];

                    triangles.Add(new Triangle(bottomLeft, bottomRight, topLeft));
                    triangles.Add(new Triangle(bottomRight, topRight, topLeft));
                }
            }

            return triangles;
        }
    }
}
