using System.Numerics;
using WinFormsApp1.Features.Rotation;

namespace WinFormsApp1.Features.Triangulation
{
    /// <summary>
    /// Wierzchołek siatki trójkątów. Zgodnie z modelem danych z treści zadania przechowuje:
    ///   - parametry u, v, z których go wyznaczono,
    ///   - punkt P, wektory styczne Pu, Pv i wektor normalny N PRZED obrotem,
    ///   - te same wielkości PO obrocie sceny.
    ///
    /// Dane "przed obrotem" liczymy raz (przy budowaniu siatki) i już się nie zmieniają.
    /// Dane "po obrocie" liczymy zawsze od nowa z danych "przed obrotem".
    /// </summary>
    public sealed class Vertex
    {
        // --- parametry, z których wyznaczono wierzchołek ---
        public float U { get; }
        public float V { get; }

        // --- przed obrotem ---
        public Vector3 Position { get; }
        public Vector3 TangentU { get; }
        public Vector3 TangentV { get; }
        public Vector3 Normal { get; }

        // --- po obrocie ---
        public Vector3 RotatedPosition { get; private set; }
        public Vector3 RotatedTangentU { get; private set; }
        public Vector3 RotatedTangentV { get; private set; }
        public Vector3 RotatedNormal { get; private set; }

        /// <summary>
        /// Wektor normalny ze wzoru z treści: N = Pu × Pv.
        /// Na starcie (bez obrotu) dane "po obrocie" są równe danym "przed obrotem",
        /// tylko N po obrocie jest od razu znormalizowany (tak jak po każdym obrocie).
        /// </summary>
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

        /// <summary>
        /// Obraca wierzchołek (treść zadania): punkt P obracamy wokół środka sceny,
        /// a Pu, Pv i N jako wektory kierunkowe. Po obrocie normalizujemy N.
        /// </summary>
        public void ApplyRotation(SceneOrientation orientation)
        {
            RotatedPosition = orientation.RotatePoint(Position);
            RotatedTangentU = orientation.RotateDirection(TangentU);
            RotatedTangentV = orientation.RotateDirection(TangentV);
            RotatedNormal = SafeNormalize(orientation.RotateDirection(Normal));
        }

        /// <summary>
        /// Normalizacja odporna na wektor zerowy. Gdyby Pu i Pv były równoległe
        /// (np. kilka punktów kontrolnych w jednym miejscu), Pu × Pv = 0 i nie da się go
        /// znormalizować - wtedy zakładamy normalną skierowaną w górę (+z).
        /// </summary>
        private static Vector3 SafeNormalize(Vector3 vector)
        {
            const float MinLength = 1e-6f;
            return vector.Length() < MinLength ? Vector3.UnitZ : Vector3.Normalize(vector);
        }
    }
}
