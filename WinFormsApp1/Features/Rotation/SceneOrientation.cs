using System.Numerics;
using WinFormsApp1.Mathematics;

namespace WinFormsApp1.Features.Rotation
{
    /// <summary>
    /// Aktualna orientacja (obrót) całej sceny, wokół środka sceny (0, 0, 0).
    ///
    /// Obrót jest NARASTAJĄCY (wymaganie z treści zadania): nie trzymamy kątów,
    /// tylko jedną macierz obrotu R. Każdy mały obrót (np. z ruchu myszy) doklejamy do niej:
    ///     R_nowa = R_przyrost · R_aktualna
    /// Przyrost jest liczony wokół osi EKRANU (x w prawo, y w górę, z do nas),
    /// więc działa tak samo intuicyjnie niezależnie od tego, jak scena jest już obrócona.
    ///
    /// Używamy własnej macierzy Matrix3 (wektory kolumnowe, P' = R · P - jak na wykładzie).
    /// </summary>
    public sealed class SceneOrientation
    {
        /// <summary>Macierz obrotu sceny: punkt po obrocie = Rotation · punkt.</summary>
        public Matrix3 Rotation { get; private set; } = Matrix3.Identity;

        /// <summary>Obrót wokół poziomej osi ekranu (x). Kąt w radianach.</summary>
        public void RotateAroundScreenX(float angle) => ApplyIncrement(Matrix3.RotationX(angle));

        /// <summary>Obrót wokół pionowej osi ekranu (y). Kąt w radianach.</summary>
        public void RotateAroundScreenY(float angle) => ApplyIncrement(Matrix3.RotationY(angle));

        /// <summary>Obrót wokół osi z (prostopadłej do ekranu). Kąt w radianach.</summary>
        public void RotateAroundScreenZ(float angle) => ApplyIncrement(Matrix3.RotationZ(angle));

        /// <summary>Wraca do orientacji bez obrotu (widok prosto z góry).</summary>
        public void Reset() => Rotation = Matrix3.Identity;

        /// <summary>
        /// Widok startowy "z ukosa", żeby od razu było widać, że powierzchnie mają różne z:
        /// najpierw obrót o 30° wokół osi z, potem pochylenie o 55° wokół poziomej osi ekranu
        /// (górna krawędź sceny odchyla się od nas).
        /// </summary>
        public void SetDefaultView()
        {
            Reset();
            RotateAroundScreenZ(DegreesToRadians(30));
            RotateAroundScreenX(DegreesToRadians(-55));
        }

        public static float DegreesToRadians(float degrees) => degrees * MathF.PI / 180f;

        /// <summary>
        /// Obraca PUNKT (np. punkt kontrolny, wierzchołek siatki) wokół środka sceny: P' = R · P.
        /// </summary>
        public Vector3 RotatePoint(Vector3 point) => Rotation * point;

        /// <summary>
        /// Obraca WEKTOR KIERUNKOWY (Pu, Pv, N) tym samym wzorem. Przy samym obrocie (bez przesunięcia)
        /// to ta sama operacja co dla punktu, ale osobna metoda pokazuje w kodzie, co obracamy
        /// (treść zadania rozróżnia punkty i wektory kierunkowe).
        /// </summary>
        public Vector3 RotateDirection(Vector3 direction) => Rotation * direction;

        private void ApplyIncrement(Matrix3 increment)
        {
            // R_nowa = R_przyrost · R_aktualna  (najpierw dotychczasowy obrót, potem nowy przyrost)
            Matrix3 combined = increment * Rotation;

            // Usuwamy drobne błędy zaokrągleń, żeby macierz pozostała czystym obrotem.
            Rotation = combined.Orthonormalized();
        }
    }
}
