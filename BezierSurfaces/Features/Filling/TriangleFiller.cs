using System.Numerics;

namespace BezierSurfaces.Features.Filling
{
    // Wypełnianie trójkąta algorytmem scan-linii z sortowaniem wierzchołków.
    // Idzie wiersz po wierszu i trzyma listę krawędzi przecinających bieżący wiersz (AET).
    public static class TriangleFiller
    {
        private const int VertexCount = 3;

        // Wypełnia trójkąt o wierzchołkach a, b, c.
        // Dla każdego poziomego odcinka wnętrza woła fillSpan.
        public static void Fill(Vector2 a, Vector2 b, Vector2 c, FillSpan fillSpan)
        {
            Vector2[] vertices = { a, b, c };

            int[] sortedByY = SortIndicesByY(vertices);

            List<ActiveEdge> activeEdges = new List<ActiveEdge>();

            int firstRow = CeilToInt(vertices[sortedByY[0]].Y);
            int lastRow = CeilToInt(vertices[sortedByY[VertexCount - 1]].Y) - 1;

            int nextSortedVertex = 0;

            // kolejne scan-linie od góry do dołu w pętli.
            for (int y = firstRow; y <= lastRow; y++)
            {
                // Gdy scan-linia minie wierzchołek, aktualizujemy krawędzie, które się w nim zaczynają lub kończą.
                while (nextSortedVertex < VertexCount && CeilToInt(vertices[sortedByY[nextSortedVertex]].Y) <= y)
                {
                    UpdateEdgesAtVertex(vertices, sortedByY[nextSortedVertex], y, activeEdges);
                    nextSortedVertex++;
                }

                // Krawędzie posortowane od lewej do prawej.
                activeEdges.Sort((first, second) => first.X.CompareTo(second.X));

                // Zamalowujemy piksele między kolejnymi parami krawędzi.
                for (int pair = 0; pair + 1 < activeEdges.Count; pair += 2)
                {
                    int xFrom = CeilToInt(activeEdges[pair].X);
                    int xTo = CeilToInt(activeEdges[pair + 1].X) - 1;

                    if (xFrom <= xTo)
                    {
                        fillSpan(y, xFrom, xTo);
                    }
                }

                // Przejście do następnego wiersza - przesuwamy x każdej krawędzi.
                foreach (ActiveEdge edge in activeEdges)
                {
                    edge.X += edge.InverseSlope;
                }
            }
        }

        // Sprawdzamy krawędzie do sąsiadów wierzchołka i aktualizujemy listę aktywnych krawędzi (AET).
        private static void UpdateEdgesAtVertex(Vector2[] vertices, int vertex, int y, List<ActiveEdge> activeEdges)
        {
            int previous = (vertex + VertexCount - 1) % VertexCount;
            int next = (vertex + 1) % VertexCount;

            UpdateEdge(vertices, vertex, previous, y, activeEdges);
            UpdateEdge(vertices, vertex, next, y, activeEdges);
        }

        // Krawędź do sąsiada niżej właśnie się zaczyna, więc dodajemy ją do listy.
        // Krawędź do sąsiada wyżej właśnie się skończyła, więc ją usuwamy.
        // Krawędź poziomą pomijamy.
        private static void UpdateEdge(Vector2[] vertices, int vertex, int neighbour, int y, List<ActiveEdge> activeEdges)
        {
            Vector2 current = vertices[vertex];
            Vector2 other = vertices[neighbour];

            // Sąsiad jest niżej na ekranie
            if (other.Y > current.Y)
            {
                // Krawędź tak krótka, że kończy się przed następnym wierszem, nie przecina żadnej scan-linii.
                bool edgeCoversAnyRow = CeilToInt(other.Y) > y;
                if (edgeCoversAnyRow)
                {
                    // Pochylenie krawędzi i jej x od razu w bieżącym wierszu, bo wierzchołek może leżeć między wierszami.
                    float inverseSlope = (other.X - current.X) / (other.Y - current.Y);
                    float xAtThisRow = current.X + (y - current.Y) * inverseSlope;
                    activeEdges.Add(new ActiveEdge(vertex, neighbour, xAtThisRow, inverseSlope));
                }
            }

            // Sąsiad jest wyżej, czyli krawędź się skończyła.
            else if (other.Y < current.Y)
            {
                activeEdges.RemoveAll(edge => edge.UpperVertex == neighbour && edge.LowerVertex == vertex);
            }
        }

        // Sortuje trzy wierzchołki według y trzema porównaniami z zamianą. Zwraca numery wierzchołków.
        private static int[] SortIndicesByY(Vector2[] vertices)
        {
            int first = 0;
            int second = 1;
            int third = 2;

            if (vertices[second].Y < vertices[first].Y)
            {
                (first, second) = (second, first);
            }

            if (vertices[third].Y < vertices[second].Y)
            {
                (second, third) = (third, second);
            }

            if (vertices[second].Y < vertices[first].Y)
            {
                (first, second) = (second, first);
            }

            return new[] { first, second, third };
        }

        private static int CeilToInt(float value) => (int)MathF.Ceiling(value);
    }
}
