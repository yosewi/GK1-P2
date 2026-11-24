using System.Numerics;
using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Mathematics;

namespace BezierSurfaces.Features.Triangulation
{
    // Wierzchołek siatki trójkątów.
    // Trzyma parametry u i v oraz punkt, wektory styczne i wektor normalny przed obrotem i po obrocie.
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
            RotatedNormal = VectorMath.SafeNormalize(Normal);
        }

        // Obraca wierzchołek bieżącym obrotem sceny, zawsze zaczynając od wartości oryginalnych.
        public void ApplyRotation(SceneOrientation orientation)
        {
            RotatedPosition = orientation.RotatePoint(Position);
            RotatedTangentU = orientation.RotateDirection(TangentU);
            RotatedTangentV = orientation.RotateDirection(TangentV);

            // Normalną po obrocie skracamy do długości 1, bo taką będzie potrzebować oświetlenie.
            RotatedNormal = VectorMath.SafeNormalize(orientation.RotateDirection(Normal));
        }
    }
}
