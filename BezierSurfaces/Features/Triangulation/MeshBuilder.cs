using System.Numerics;
using BezierSurfaces.Features.Surfaces;

namespace BezierSurfaces.Features.Triangulation
{
    public static class MeshBuilder
    {
        public const int MinSteps = 1;

        public static TriangleMesh Build(BezierSurface surface, int steps)
        {
            if (steps < MinSteps)
            {
                throw new ArgumentOutOfRangeException(nameof(steps), $"Steps must be at least {MinSteps}.");
            }

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
