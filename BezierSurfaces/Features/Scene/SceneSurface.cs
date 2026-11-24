using System.Numerics;
using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.Features.Texturing;
using BezierSurfaces.Features.Triangulation;

namespace BezierSurfaces.Features.Scene
{
    // Jedna powierzchnia w scenie razem z jej siatką trójkątów i własnym kolorem obiektu.
    public sealed class SceneSurface
    {
        public BezierSurface Surface { get; }
        public TriangleMesh Mesh { get; private set; }

        public Vector3 ObjectColor { get; set; }

        public Texture? Texture { get; set; }

        public bool UseTexture { get; set; }

        public Texture? ActiveTexture => UseTexture ? Texture : null;

        public SceneSurface(BezierSurface surface, int steps, Vector3 objectColor)
        {
            Surface = surface;
            Mesh = MeshBuilder.Build(surface, steps);
            ObjectColor = objectColor;
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
