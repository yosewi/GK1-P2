using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.Features.Triangulation;

namespace BezierSurfaces.Features.Scene
{
    public sealed class SceneSurface
    {
        public BezierSurface Surface { get; }
        public TriangleMesh Mesh { get; private set; }

        public SceneSurface(BezierSurface surface, int steps)
        {
            Surface = surface;
            Mesh = MeshBuilder.Build(surface, steps);
        }

        public void RebuildMesh(int steps)
        {
            Mesh = MeshBuilder.Build(Surface, steps);
        }

        public void ApplyRotation(SceneOrientation orientation)
        {
            Surface.ApplyRotation(orientation);
            Mesh.ApplyRotation(orientation);
        }
    }
}
