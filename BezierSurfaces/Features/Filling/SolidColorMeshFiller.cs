using BezierSurfaces.Features.Triangulation;
using BezierSurfaces.Rendering;

namespace BezierSurfaces.Features.Filling
{
    public static class SolidColorMeshFiller
    {
        public static void Fill(DirectBitmap frame, ScreenMapper mapper, TriangleMesh mesh, Color color)
        {
            FillSpan fillRow = (y, xFrom, xTo) => frame.FillRow(y, xFrom, xTo, color);

            foreach (Triangle triangle in mesh.Triangles)
            {
                TriangleFiller.Fill(
                    mapper.ToScreen(triangle.A.RotatedPosition),
                    mapper.ToScreen(triangle.B.RotatedPosition),
                    mapper.ToScreen(triangle.C.RotatedPosition),
                    fillRow);
            }
        }
    }
}
