using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.Features.Triangulation;

namespace BezierSurfaces.Features.Scene
{
    // Jedna powierzchnia w scenie razem z jej siatką trójkątów.
    public sealed class SceneSurface
    {
        public BezierSurface Surface { get; }
        public TriangleMesh Mesh { get; private set; }

        // Buduje siatkę o podanej dokładności.
        public SceneSurface(BezierSurface surface, int steps)
        {
            Surface = surface;
            Mesh = MeshBuilder.Build(surface, steps);
        }

        // Buduje siatkę od nowa, np. po zmianie dokładności suwakiem.
        public void RebuildMesh(int steps)
        {
            Mesh = MeshBuilder.Build(Surface, steps);
        }

        // Obraca punkty kontrolne (do wieloboku) i wierzchołki siatki (do siatki i wypełnienia).
        public void ApplyRotation(SceneOrientation orientation)
        {
            Surface.ApplyRotation(orientation);
            Mesh.ApplyRotation(orientation);
        }
    }
}
