using System.Numerics;
using BezierSurfaces.Features.Texturing;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.NormalMapping
{
    // Mapa wektorów normalnych.
    public sealed class NormalMap
    {
        private readonly Texture _image;

        public NormalMap(Texture image)
        {
            _image = image;
        }

        public static NormalMap Load(string path)
        {
            return new NormalMap(Texture.Load(path));
        }

        // Wektor zapisany w mapie dla parametrów u, v. Kolor z zakresu 0-1 przeliczamy na zakres od -1 do 1.
        public Vector3 Sample(float u, float v)
        {
            Vector3 color = _image.Sample(u, v);
            return VectorMath.SafeNormalize(2 * color - Vector3.One);
        }

        // Zmienia wektor normalny powierzchni na podstawie mapy. z układu powierzchni na układ sceny.
        public Vector3 Apply(Vector3 surfaceNormal, Vector3 tangentU, Vector3 tangentV, float u, float v)
        {
            Matrix3 transform = Matrix3.FromColumns(
                VectorMath.SafeNormalize(tangentU),
                VectorMath.SafeNormalize(tangentV),
                surfaceNormal);

            return VectorMath.SafeNormalize(transform * Sample(u, v));
        }
    }
}
