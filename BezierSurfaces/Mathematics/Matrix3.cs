using System.Numerics;

namespace BezierSurfaces.Mathematics
{
    public readonly struct Matrix3
    {
        public readonly float R11, R12, R13;
        public readonly float R21, R22, R23;
        public readonly float R31, R32, R33;

        public Matrix3(
            float r11, float r12, float r13,
            float r21, float r22, float r23,
            float r31, float r32, float r33)
        {
            R11 = r11; R12 = r12; R13 = r13;
            R21 = r21; R22 = r22; R23 = r23;
            R31 = r31; R32 = r32; R33 = r33;
        }

        public static readonly Matrix3 Identity = new Matrix3(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1);

        public static Matrix3 FromColumns(Vector3 a, Vector3 b, Vector3 c) => new Matrix3(
            a.X, b.X, c.X,
            a.Y, b.Y, c.Y,
            a.Z, b.Z, c.Z);

        public Vector3 Column1 => new Vector3(R11, R21, R31);
        public Vector3 Column2 => new Vector3(R12, R22, R32);
        public Vector3 Column3 => new Vector3(R13, R23, R33);

        public static Matrix3 RotationX(float angle)
        {
            float c = MathF.Cos(angle);
            float s = MathF.Sin(angle);
            return new Matrix3(
                1, 0, 0,
                0, c, -s,
                0, s, c);
        }

        public static Matrix3 RotationY(float angle)
        {
            float c = MathF.Cos(angle);
            float s = MathF.Sin(angle);
            return new Matrix3(
                c, 0, s,
                0, 1, 0,
                -s, 0, c);
        }

        public static Matrix3 RotationZ(float angle)
        {
            float c = MathF.Cos(angle);
            float s = MathF.Sin(angle);
            return new Matrix3(
                c, -s, 0,
                s, c, 0,
                0, 0, 1);
        }

        public static Vector3 operator *(Matrix3 r, Vector3 p)
        {
            float x = r.R11 * p.X + r.R12 * p.Y + r.R13 * p.Z;
            float y = r.R21 * p.X + r.R22 * p.Y + r.R23 * p.Z;
            float z = r.R31 * p.X + r.R32 * p.Y + r.R33 * p.Z;
            return new Vector3(x, y, z);
        }

        public static Matrix3 operator *(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(
                a.R11 * b.R11 + a.R12 * b.R21 + a.R13 * b.R31,
                a.R11 * b.R12 + a.R12 * b.R22 + a.R13 * b.R32,
                a.R11 * b.R13 + a.R12 * b.R23 + a.R13 * b.R33,

                a.R21 * b.R11 + a.R22 * b.R21 + a.R23 * b.R31,
                a.R21 * b.R12 + a.R22 * b.R22 + a.R23 * b.R32,
                a.R21 * b.R13 + a.R22 * b.R23 + a.R23 * b.R33,

                a.R31 * b.R11 + a.R32 * b.R21 + a.R33 * b.R31,
                a.R31 * b.R12 + a.R32 * b.R22 + a.R33 * b.R32,
                a.R31 * b.R13 + a.R32 * b.R23 + a.R33 * b.R33);
        }

        public Matrix3 Orthonormalized()
        {
            Vector3 c1 = Vector3.Normalize(Column1);

            Vector3 c2 = Column2 - Vector3.Dot(Column2, c1) * c1;
            c2 = Vector3.Normalize(c2);

            Vector3 c3 = Vector3.Cross(c1, c2);

            return FromColumns(c1, c2, c3);
        }
    }
}
