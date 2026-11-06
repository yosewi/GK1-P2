using System.Numerics;
using BezierSurfaces.Features.Rotation;

namespace BezierSurfaces.Features.Triangulation
{
    public sealed class Vertex
    {
        public float U { get; }
        public float V { get; }

        public Vector3 Position { get; }
        public Vector3 TangentU { get; }
        public Vector3 TangentV { get; }
        public Vector3 Normal { get; }

        public Vector3 RotatedPosition { get; private set; }
        public Vector3 RotatedTangentU { get; private set; }
        public Vector3 RotatedTangentV { get; private set; }
        public Vector3 RotatedNormal { get; private set; }

        public Vertex(float u, float v, Vector3 position, Vector3 tangentU, Vector3 tangentV)
        {
            U = u;
            V = v;

            Position = position;
            TangentU = tangentU;
            TangentV = tangentV;
            Normal = Vector3.Cross(tangentU, tangentV);

            RotatedPosition = Position;
            RotatedTangentU = TangentU;
            RotatedTangentV = TangentV;
            RotatedNormal = SafeNormalize(Normal);
        }

        public void ApplyRotation(SceneOrientation orientation)
        {
            RotatedPosition = orientation.RotatePoint(Position);
            RotatedTangentU = orientation.RotateDirection(TangentU);
            RotatedTangentV = orientation.RotateDirection(TangentV);
            RotatedNormal = SafeNormalize(orientation.RotateDirection(Normal));
        }

        private static Vector3 SafeNormalize(Vector3 vector)
        {
            const float MinLength = 1e-6f;
            return vector.Length() < MinLength ? Vector3.UnitZ : Vector3.Normalize(vector);
        }
    }
}
