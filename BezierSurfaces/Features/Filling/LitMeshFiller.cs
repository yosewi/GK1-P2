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
            LightingModel lighting, ActiveLights lights, Vector3 objectColor, Texture? texture, NormalMap? normalMap,
            HeightMap? heightMap)
        {
            foreach (Triangle triangle in mesh.Triangles)
            {
                FillTriangle(pixels, depthBuffer, mapper, triangle, lighting, lights, objectColor, texture, normalMap, heightMap);
            }
        }

        // Wypełnia jeden trójkąt. Dla każdego piksela wnętrza wyznacza głębokość i wektor normalny
        // z wartości w wierzchołkach, a potem kolor z modelu oświetlenia.
        private static void FillTriangle(FastBitmap pixels, DepthBuffer depthBuffer, ScreenMapper mapper, Triangle triangle,
            LightingModel lighting, ActiveLights lights, Vector3 objectColor, Texture? texture, NormalMap? normalMap,
            HeightMap? heightMap)
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

            // Głębokości i wektory normalne wierzchołków po obrocie.
            float zA = triangle.A.RotatedPosition.Z;
            float zB = triangle.B.RotatedPosition.Z;
            float zC = triangle.C.RotatedPosition.Z;

            Vector3 normalA = triangle.A.RotatedNormal;
            Vector3 normalB = triangle.B.RotatedNormal;
            Vector3 normalC = triangle.C.RotatedNormal;

            // Wektory styczne wierzchołków po obrocie
            Vector3 tangentUA = triangle.A.RotatedTangentU;
            Vector3 tangentUB = triangle.B.RotatedTangentU;
            Vector3 tangentUC = triangle.C.RotatedTangentU;

            Vector3 tangentVA = triangle.A.RotatedTangentV;
            Vector3 tangentVB = triangle.B.RotatedTangentV;
            Vector3 tangentVC = triangle.C.RotatedTangentV;

            // Parametry u, v wierzchołków
            Vector2 parametersA = new Vector2(triangle.A.U, triangle.A.V);
            Vector2 parametersB = new Vector2(triangle.B.U, triangle.B.V);
            Vector2 parametersC = new Vector2(triangle.C.U, triangle.C.V);
            bool needsTangents = normalMap != null || heightMap != null;
            bool needsParameters = texture != null || needsTangents;

            TriangleFiller.Fill(a, b, c, (y, xFrom, xTo) =>
            {
                for (int x = xFrom; x <= xTo; x++)
                {
                    Vector3 weights = barycentric.Compute(x, y);
                    float z = BarycentricCoordinates.Interpolate(weights, zA, zB, zC);

                    if (!depthBuffer.TryUpdate(x, y, z))
                    {
                        continue;
                    }

                    // Parametry u, v tego piksela
                    Vector2 parameters = needsParameters
                        ? BarycentricCoordinates.Interpolate(weights, parametersA, parametersB, parametersC)
                        : Vector2.Zero;

                    // Wektor normalny w tym pikselu.
                    Vector3 normal = VectorMath.SafeNormalize(
                        BarycentricCoordinates.Interpolate(weights, normalA, normalB, normalC));

                    bool isOnOtherSide = normal.Z < 0;

                    // Mapa normalnych albo mapa wysokości zmienia normalną w miejscu (u, v).
                    if (needsTangents)
                    {
                        Vector3 tangentU = BarycentricCoordinates.Interpolate(weights, tangentUA, tangentUB, tangentUC);
                        Vector3 tangentV = BarycentricCoordinates.Interpolate(weights, tangentVA, tangentVB, tangentVC);

                        if (normalMap != null)
                        {
                            normal = normalMap.Apply(normal, tangentU, tangentV, parameters.X, parameters.Y);
                        }
                        else if (heightMap != null)
                        {
                            normal = heightMap.Apply(normal, tangentU, tangentV, parameters.X, parameters.Y);
                        }
                    }

                    if(isOnOtherSide)
                    {
                        normal = -normal;
                    }

                    // Punkt sceny widoczny w tym pikselu.
                    Vector3 point = mapper.ToWorld(x, y, z);

                    Vector3 pixelObjectColor = objectColor;
                    if (texture != null)
                    {
                        pixelObjectColor = texture.Sample(parameters.X, parameters.Y);
                    }

                    // Suma po wszystkich światłach
                    Vector3 color = lighting.Compute(normal, point, lights, pixelObjectColor);
                    pixels.SetPixel(x, y, LightingModel.ToColor(color));
                }
            });
        }
    }
}