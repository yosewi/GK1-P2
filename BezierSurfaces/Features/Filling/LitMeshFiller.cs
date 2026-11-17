using System.Numerics;
using FastBitmapLib;
using BezierSurfaces.Features.Lighting;
using BezierSurfaces.Features.NormalMapping;
using BezierSurfaces.Features.Texturing;
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
            LightingModel lighting, Vector3 lightPosition, Vector3 objectColor, Texture? texture, NormalMap? normalMap)
        {
            foreach (Triangle triangle in mesh.Triangles)
            {
                FillTriangle(pixels, depthBuffer, mapper, triangle, lighting, lightPosition, objectColor, texture, normalMap);
            }
        }

        // Wypełnia jeden trójkąt. Dla każdego piksela wnętrza wyznacza głębokość i wektor normalny
        // z wartości w wierzchołkach, a potem kolor z modelu oświetlenia.
        private static void FillTriangle(FastBitmap pixels, DepthBuffer depthBuffer, ScreenMapper mapper, Triangle triangle,
            LightingModel lighting, Vector3 lightPosition, Vector3 objectColor, Texture? texture, NormalMap? normalMap)
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

            // Wektory styczne wierzchołków po obrocie - potrzebne przy mapie normalnych.
            Vector3 tangentUA = triangle.A.RotatedTangentU;
            Vector3 tangentUB = triangle.B.RotatedTangentU;
            Vector3 tangentUC = triangle.C.RotatedTangentU;

            Vector3 tangentVA = triangle.A.RotatedTangentV;
            Vector3 tangentVB = triangle.B.RotatedTangentV;
            Vector3 tangentVC = triangle.C.RotatedTangentV;

            // Parametry u, v wierzchołków - z nich wyznaczamy miejsce na teksturze i na mapie normalnych.
            Vector2 parametersA = new Vector2(triangle.A.U, triangle.A.V);
            Vector2 parametersB = new Vector2(triangle.B.U, triangle.B.V);
            Vector2 parametersC = new Vector2(triangle.C.U, triangle.C.V);
            bool needsParameters = texture != null || normalMap != null;

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

                    // Parametry u, v tego piksela
                    Vector2 parameters = needsParameters
                        ? BarycentricCoordinates.Interpolate(weights, parametersA, parametersB, parametersC)
                        : Vector2.Zero;

                    // Wektor normalny w tym pikselu. Po uśrednieniu jest krótszy niż 1, więc go normalizujemy.
                    Vector3 normal = VectorMath.SafeNormalize(
                        BarycentricCoordinates.Interpolate(weights, normalA, normalB, normalC));

                    // Z mapą normalnych: normalną zmieniamy wektorem odczytanym z mapy w miejscu (u, v).
                    if (normalMap != null)
                    {
                        Vector3 tangentU = BarycentricCoordinates.Interpolate(weights, tangentUA, tangentUB, tangentUC);
                        Vector3 tangentV = BarycentricCoordinates.Interpolate(weights, tangentVA, tangentVB, tangentVC);
                        normal = normalMap.Apply(normal, tangentU, tangentV, parameters.X, parameters.Y);
                    }

                    // Punkt sceny widoczny w tym pikselu i kierunek od niego do światła.
                    Vector3 point = mapper.ToWorld(x, y, z);
                    Vector3 toLight = VectorMath.SafeNormalize(lightPosition - point);

                    // Kolor obiektu w tym pikselu: z tekstury w miejscu (u, v) albo stały.
                    Vector3 pixelObjectColor = objectColor;
                    if (texture != null)
                    {
                        pixelObjectColor = texture.Sample(parameters.X, parameters.Y);
                    }

                    Vector3 color = lighting.Compute(normal, toLight, pixelObjectColor);
                    pixels.SetPixel(x, y, LightingModel.ToColor(color));
                }
            });
        }
    }
}