namespace WinFormsApp1.Features.Triangulation
{
    /// <summary>
    /// Trójkąt siatki: trzy wierzchołki (treść zadania: "trójkąt - trzy wierzchołki").
    /// Trzymamy REFERENCJE do wierzchołków, więc sąsiednie trójkąty dzielą te same obiekty Vertex
    /// - wierzchołek obracamy raz, a wszystkie jego trójkąty od razu widzą wynik.
    /// </summary>
    public sealed class Triangle
    {
        public Vertex A { get; }
        public Vertex B { get; }
        public Vertex C { get; }

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}
