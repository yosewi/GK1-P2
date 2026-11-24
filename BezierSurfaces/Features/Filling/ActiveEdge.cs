namespace BezierSurfaces.Features.Filling
{
    // Krawędź z listy aktywnych krawędzi (AET), czyli krawędź przecinana przez bieżącą scan-linię.
    public sealed class ActiveEdge
    {
        // Numery wierzchołków na górnym i dolnym końcu krawędzi.
        public int UpperVertex { get; }
        public int LowerVertex { get; }

        // Miejsce, w którym krawędź przecina bieżącą scan-linię.
        public float X { get; set; }

        // O ile przesuwa się x krawędzi przy przejściu o jeden wiersz w dół.
        public float InverseSlope { get; }

        public ActiveEdge(int upperVertex, int lowerVertex, float x, float inverseSlope)
        {
            UpperVertex = upperVertex;
            LowerVertex = lowerVertex;
            X = x;
            InverseSlope = inverseSlope;
        }
    }
}
