namespace BezierSurfaces.Features.Triangulation
{
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
