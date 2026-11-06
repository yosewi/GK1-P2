using System.Numerics;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.Rotation
{
    public sealed class SceneOrientation
    {
        public Matrix3 Rotation { get; private set; } = Matrix3.Identity;

        public void RotateAroundScreenX(float angle) => ApplyIncrement(Matrix3.RotationX(angle));

        public void RotateAroundScreenY(float angle) => ApplyIncrement(Matrix3.RotationY(angle));

        public void RotateAroundScreenZ(float angle) => ApplyIncrement(Matrix3.RotationZ(angle));

        public void Reset() => Rotation = Matrix3.Identity;

        public void SetDefaultView()
        {
            Reset();
            RotateAroundScreenZ(DegreesToRadians(30));
            RotateAroundScreenX(DegreesToRadians(-55));
        }

        public static float DegreesToRadians(float degrees) => degrees * MathF.PI / 180f;

        public Vector3 RotatePoint(Vector3 point) => Rotation * point;

        public Vector3 RotateDirection(Vector3 direction) => Rotation * direction;

        private void ApplyIncrement(Matrix3 increment)
        {
            Matrix3 combined = increment * Rotation;

            Rotation = combined.Orthonormalized();
        }
    }
}
