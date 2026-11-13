using System.Numerics;
using FastBitmapLib;
using BezierSurfaces.Features.Lighting;
using BezierSurfaces.Features.Triangulation;
using BezierSurfaces.Mathematics;
using BezierSurfaces.Rendering;

namespace BezierSurfaces.Features.Filling
{
    // Wypełnia siatkę trójkątów kolorem z modelu oświetlenia, z uwzględnieniem Z-bufora.
    public static class LitMeshFiller
    {
        // Każdy trójkąt siatki wypełniamy niezależnie.
        public static void Fill(FastBitmap pixels, DepthBuffer depthBuffer, ScreenMapper mapper, TriangleMesh mesh,
            LightingModel lighting, Vector3 lightPosition, Vector3 objectColor)
        {
            foreach (Triangle triangle in mesh.Triangles)
            {
                FillTriangle(pixels, depthBuffer, mapper, triangle, lighting, lightPosition, objectColor);
            }
        }

        // Wypełnia jeden trójkąt. Dla każdego piksela wnętrza wyznacza głębokość i wektor normalny
        // z wartości w wierzchołkach, a potem kolor z modelu oświetlenia.
        private static void FillTriangle(FastBitmap pixels, DepthBuffer depthBuffer, ScreenMapper mapper, Triangle triangle,
            LightingModel lighting, Vector3 lightPosition, Vector3 objectColor)
        {
            // Wierzchołki po obrocie zamienione na piksele ekranu.
            Vector2 a = mapper.ToScreen(triangle.A.RotatedPosition);
            Vector2 b = mapper.ToScreen(triangle.B.RotatedPosition);
            Vector2 c = mapper.ToScreen(triangle.C.RotatedPosition);

            BarycentricCoordinates barycentric = new BarycentricCoordinates(a, b, c);

            // Trójkąt widziany z boku (zerowe pole) nie ma wnętrza, więc pomijamy go.
            if (barycentric.IsDegenerate)
            {
                return;
            }

            // Głębokości i wektory normalne wierzchołków po obrocie.
            float zA = triangle.A.RotatedPosition.Z;
            float zB = triangle.B.RotatedPosition.Z;
            float zC = triangle.C.RotatedPosition.Z;

            Vector3 normalA = triangle.A.RotatedNormal;
            Vector3 normalB = triangle.B.RotatedNormal;
            Vector3 normalC = triangle.C.RotatedNormal;

            TriangleFiller.Fill(a, b, c, (y, xFrom, xTo) =>
            {
                for (int x = xFrom; x <= xTo; x++)
                {
                    Vector3 weights = barycentric.Compute(x, y);
                    float z = BarycentricCoordinates.Interpolate(weights, zA, zB, zC);

                    // Piksel zasłonięty przez coś bliższego - nie liczymy dla niego koloru.
                    if (!depthBuffer.TryUpdate(x, y, z))
                    {
                        continue;
                    }

                    // Wektor normalny w tym pikselu. Po uśrednieniu jest krótszy niż 1, więc go normalizujemy.
                    Vector3 normal = VectorMath.SafeNormalize(
                        BarycentricCoordinates.Interpolate(weights, normalA, normalB, normalC));

                    // Punkt sceny widoczny w tym pikselu i kierunek od niego do światła.
                    Vector3 point = mapper.ToWorld(x, y, z);
                    Vector3 toLight = VectorMath.SafeNormalize(lightPosition - point);

                    Vector3 color = lighting.Compute(normal, toLight, objectColor);
                    pixels.SetPixel(x, y, LightingModel.ToColor(color));
                }
            });
        }
    }
}