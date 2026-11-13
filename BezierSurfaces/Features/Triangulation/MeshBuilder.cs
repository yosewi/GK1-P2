using System.Numerics;
using BezierSurfaces.Features.Surfaces;

namespace BezierSurfaces.Features.Triangulation
{
    // Buduje siatkę trójkątów z powierzchni Beziera
    public static class MeshBuilder
    {
        // Najmniejsza podział na trójkąty
        public const int MinSteps = 1;

        // Dzieli kwadrat parametrów u, v na steps x steps małych kwadratów, a każdy z nich na dwa trójkąty.
        public static TriangleMesh Build(BezierSurface surface, int steps)
        {
            Vertex[,] vertices = BuildVertices(surface, steps);
            List<Triangle> triangles = BuildTriangles(vertices, steps);

            return new TriangleMesh(triangles);
        }

        // Liczy wierzchołki w równych odstępach u i v, po steps + 1 na każdy bok.
        private static Vertex[,] BuildVertices(BezierSurface surface, int steps)
        {
            int count = steps + 1;
            Vertex[,] vertices = new Vertex[count, count];

            for (int i = 0; i < count; i++)
            {
                // Parametry u i v idą od 0 do 1 równymi krokami.
                float u = (float)i / steps;

                for (int j = 0; j < count; j++)
                {
                    float v = (float)j / steps;

                    // Punkt powierzchni i oba wektory styczne w tym miejscu.
                    Vector3 position = surface.Point(u, v);
                    Vector3 tangentU = surface.TangentU(u, v);
                    Vector3 tangentV = surface.TangentV(u, v);

                    vertices[i, j] = new Vertex(u, v, position, tangentU, tangentV);
                }
            }

            return vertices;
        }

        // Każdy mały kwadrat siatki dzielimy przekątną na dwa trójkąty.
        private static List<Triangle> BuildTriangles(Vertex[,] vertices, int steps)
        {
            List<Triangle> triangles = new List<Triangle>(2 * steps * steps);

            for (int i = 0; i < steps; i++)
            {
                for (int j = 0; j < steps; j++)
                {
                    // Cztery narożniki bieżącego kwadratu.
                    Vertex bottomLeft = vertices[i, j];
                    Vertex bottomRight = vertices[i + 1, j];
                    Vertex topLeft = vertices[i, j + 1];
                    Vertex topRight = vertices[i + 1, j + 1];

                    // Dwa trójkąty o wspólnej przekątnej
                    triangles.Add(new Triangle(bottomLeft, bottomRight, topLeft));
                    triangles.Add(new Triangle(bottomRight, topRight, topLeft));
                }
            }

            return triangles;
        }
    }
}
