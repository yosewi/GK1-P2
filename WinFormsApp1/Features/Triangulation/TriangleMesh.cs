using WinFormsApp1.Features.Rotation;

namespace WinFormsApp1.Features.Triangulation
{
    /// <summary>
    /// Siatka trójkątów jednej powierzchni (treść zadania: "siatka - lista trójkątów").
    /// </summary>
    public sealed class TriangleMesh
    {
        public List<Triangle> Triangles { get; }

        public TriangleMesh(List<Triangle> triangles)
        {
            Triangles = triangles;
        }

        /// <summary>
        /// Obraca wszystkie wierzchołki siatki według aktualnej orientacji sceny.
        ///
        /// Sąsiednie trójkąty dzielą wierzchołki, więc ten sam wierzchołek zostanie obrócony
        /// kilka razy (raz dla każdego swojego trójkąta). Wynik jest zawsze ten sam, bo
        /// obrót liczymy z danych PRZED obrotem, a nie z poprzedniego wyniku.
        /// </summary>
        public void ApplyRotation(SceneOrientation orientation)
        {
            foreach (Triangle triangle in Triangles)
            {
                triangle.A.ApplyRotation(orientation);
                triangle.B.ApplyRotation(orientation);
                triangle.C.ApplyRotation(orientation);
            }
        }
    }
}
