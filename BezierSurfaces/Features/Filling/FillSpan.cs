namespace BezierSurfaces.Features.Filling
{
    // Funkcja wołana dla każdego poziomego odcinka do zamalowania: wiersz y, piksele od xFrom do xTo.
    // Algorytm wypełniania mówi tylko, które piksele są w trójkącie, a ta funkcja decyduje, jak je pomalować.
    public delegate void FillSpan(int y, int xFrom, int xTo);
}
