using BezierSurfaces.Features.Rotation;

namespace BezierSurfaces.Features.Triangulation
{
    // Siatka trójkątów
    public sealed class TriangleMesh
    {
        public List<Triangle> Triangles { get; }

        public TriangleMesh(List<Triangle> triangles)
        {
            Triangles = triangles;
        }

        // Obraca wszystkie wierzchołki siatki. Wspólny wierzchołek jest obracany kilka razy,
        // ale zawsze z oryginału, więc wynik jest ten sam i jest git
        public void ApplyRotation(SceneOrientation orientation)
        {
            foreach (Triangle triangle in Triangles)
            {
                triangle.A.ApplyRotation(orientation);
                triangle.B.ApplyRotation(orientation);
                triangle.C.ApplyRotation(orientation);
            }
        }
    }
}
