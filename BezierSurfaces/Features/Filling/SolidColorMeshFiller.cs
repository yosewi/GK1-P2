using System.Numerics;
using FastBitmapLib;
using BezierSurfaces.Features.Triangulation;
using BezierSurfaces.Mathematics;
using BezierSurfaces.Rendering;

namespace BezierSurfaces.Features.Filling
{
    // Wypełnia siatkę trójkątów jednym kolorem, z uwzględnieniem Z-bufora.
    public static class SolidColorMeshFiller
    {
        // Każdy trójkąt siatki wypełniamy niezależnie.
        public static void Fill(FastBitmap pixels, DepthBuffer depthBuffer, ScreenMapper mapper, TriangleMesh mesh, Color color)
        {
            foreach (Triangle triangle in mesh.Triangles)
            {
                FillTriangle(pixels, depthBuffer, mapper, triangle, color);
            }
        }

        // Wypełnia jeden trójkąt: wybiera piksele scan-linią, dla każdego liczy głębokość
        // i maluje go tylko wtedy, gdy jest bliżej niż to, co już jest w tym pikselu.
        private static void FillTriangle(FastBitmap pixels, DepthBuffer depthBuffer, ScreenMapper mapper, Triangle triangle, Color color)
        {
            // Wierzchołki po obrocie zamienione na piksele ekranu.
            Vector2 a = mapper.ToScreen(triangle.A.RotatedPosition);
            Vector2 b = mapper.ToScreen(triangle.B.RotatedPosition);
            Vector2 c = mapper.ToScreen(triangle.C.RotatedPosition);

            BarycentricCoordinates barycentric = new BarycentricCoordinates(a, b, c);

            if (barycentric.IsDegenerate)
            {
                return;
            }

            // Głębokości wierzchołków po obrocie.
            float zA = triangle.A.RotatedPosition.Z;
            float zB = triangle.B.RotatedPosition.Z;
            float zC = triangle.C.RotatedPosition.Z;

            // Dla każdego piksela wnętrza: wagi barycentryczne, z nich głębokość, test Z-bufora i ewentualnie zapis piksela.
            TriangleFiller.Fill(a, b, c, (y, xFrom, xTo) =>
            {
                for (int x = xFrom; x <= xTo; x++)
                {
                    Vector3 weights = barycentric.Compute(x, y);
                    float z = BarycentricCoordinates.Interpolate(weights, zA, zB, zC);

                    if (depthBuffer.TryUpdate(x, y, z))
                    {
                        pixels.SetPixel(x, y, color);
                    }
                }
            });
        }
    }
}
