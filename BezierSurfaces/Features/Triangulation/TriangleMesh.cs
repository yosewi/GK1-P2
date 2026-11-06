using BezierSurfaces.Features.Rotation;

namespace BezierSurfaces.Features.Triangulation
{
    public sealed class TriangleMesh
    {
        public List<Triangle> Triangles { get; }

        public TriangleMesh(List<Triangle> triangles)
        {
            Triangles = triangles;
        }

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
