using System.Numerics;
using BezierSurfaces.Features.Triangulation;
using BezierSurfaces.Mathematics;
using BezierSurfaces.Rendering;

namespace BezierSurfaces.Features.Filling
{
    public static class SolidColorMeshFiller
    {
        public static void Fill(DirectBitmap frame, DepthBuffer depthBuffer, ScreenMapper mapper, TriangleMesh mesh, Color color)
        {
            foreach (Triangle triangle in mesh.Triangles)
            {
                FillTriangle(frame, depthBuffer, mapper, triangle, color);
            }
        }

        private static void FillTriangle(DirectBitmap frame, DepthBuffer depthBuffer, ScreenMapper mapper, Triangle triangle, Color color)
        {
            Vector2 a = mapper.ToScreen(triangle.A.RotatedPosition);
            Vector2 b = mapper.ToScreen(triangle.B.RotatedPosition);
            Vector2 c = mapper.ToScreen(triangle.C.RotatedPosition);

            BarycentricCoordinates barycentric = new BarycentricCoordinates(a, b, c);
            if (barycentric.IsDegenerate)
            {
                return;
            }

            float zA = triangle.A.RotatedPosition.Z;
            float zB = triangle.B.RotatedPosition.Z;
            float zC = triangle.C.RotatedPosition.Z;

            TriangleFiller.Fill(a, b, c, (y, xFrom, xTo) =>
            {
                for (int x = xFrom; x <= xTo; x++)
                {
                    Vector3 weights = barycentric.Compute(x, y);
                    float z = BarycentricCoordinates.Interpolate(weights, zA, zB, zC);

                    if (depthBuffer.TryUpdate(x, y, z))
                    {
                        frame.SetPixel(x, y, color);
                    }
                }
            });
        }
    }
}
