using System.Numerics;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.Lighting
{
    // Światła: rodzaj, położenia i (dla reflektorów) osie D.
    public sealed class ActiveLights
    {
        public static readonly Vector3 DirectionalL = Vector3.UnitZ;

        public static readonly Vector3 ReflectorTarget = Vector3.Zero;

        public LightType Type { get; }

        public IReadOnlyList<Vector3> Positions { get; }

        public IReadOnlyList<Vector3> ReflectorAxes { get; }

        public int ReflectorM { get; }

        public ActiveLights(LightType type, IReadOnlyList<Vector3> positions, int reflectorM)
        {
            Type = type;
            Positions = positions;
            ReflectorM = reflectorM;
            ReflectorAxes = positions.Select(position => VectorMath.SafeNormalize(ReflectorTarget - position)).ToList();
        }

        // Jaka część światła numer index dociera do punktu, z którego kierunek do światła to toLight.
        public float ReflectorFactor(int index, Vector3 toLight)
        {
            if (Type != LightType.Reflector)
            {
                return 1f;
            }

            float cosine = Vector3.Dot(-toLight, ReflectorAxes[index]);
            return MathF.Pow(MathF.Max(0, cosine), ReflectorM);
        }
    }
}
