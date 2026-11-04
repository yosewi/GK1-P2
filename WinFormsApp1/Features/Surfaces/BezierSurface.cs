using System.Numerics;
using WinFormsApp1.Features.Rotation;

namespace WinFormsApp1.Features.Surfaces
{
    /// <summary>
    /// Powierzchnia Béziera trzeciego stopnia (bikubiczna).
    /// Jest opisana przez 16 punktów kontrolnych ułożonych w siatkę 4 x 4.
    ///
    /// ControlPoints[i, j]:
    ///   i - numer wiersza siatki, kierunek parametru u (u nas: oś x),
    ///   j - numer kolumny siatki, kierunek parametru v (u nas: oś y).
    ///
    /// Na razie klasa tylko przechowuje punkty. Wzory na punkt powierzchni P(u, v)
    /// i wektory styczne Pu, Pv dojdą w etapie triangulacji.
    /// </summary>
    public sealed class BezierSurface
    {
        /// <summary>Stopień powierzchni w każdym kierunku (n = m = 3).</summary>
        public const int Degree = 3;

        /// <summary>Liczba punktów kontrolnych w jednym kierunku (stopień + 1 = 4).</summary>
        public const int PointsPerSide = Degree + 1;

        /// <summary>Liczba wszystkich punktów kontrolnych (4 * 4 = 16).</summary>
        public const int ControlPointCount = PointsPerSide * PointsPerSide;

        /// <summary>
        /// Punkty kontrolne V[i, j] przed obrotem (dokładnie tak, jak w pliku).
        /// </summary>
        public Vector3[,] ControlPoints { get; }

        /// <summary>
        /// Te same punkty kontrolne PO obrocie sceny. Z nich rysujemy wielobok Béziera.
        /// Liczymy je zawsze od nowa z ControlPoints (oryginałów), nigdy z poprzednich obróconych,
        /// żeby błędy zaokrągleń się nie kumulowały.
        /// </summary>
        public Vector3[,] RotatedControlPoints { get; }

        /// <summary>
        /// Tworzy powierzchnię z 16 punktów w kolejności z pliku:
        /// V00, V01, V02, V03, V10, V11, ... , V33.
        /// Punkt numer k trafia do ControlPoints[k / 4, k % 4].
        /// </summary>
        public BezierSurface(IReadOnlyList<Vector3> pointsInFileOrder)
        {
            if (pointsInFileOrder.Count != ControlPointCount)
            {
                throw new ArgumentException(
                    $"A Bezier surface needs exactly {ControlPointCount} control points, got {pointsInFileOrder.Count}.");
            }

            ControlPoints = new Vector3[PointsPerSide, PointsPerSide];

            for (int k = 0; k < ControlPointCount; k++)
            {
                int i = k / PointsPerSide; // wiersz siatki
                int j = k % PointsPerSide; // kolumna siatki
                ControlPoints[i, j] = pointsInFileOrder[k];
            }

            // Na początku (bez obrotu) punkty obrócone = oryginalne.
            RotatedControlPoints = (Vector3[,])ControlPoints.Clone();
        }

        /// <summary>
        /// Przelicza punkty kontrolne po obrocie: obrócony = obrót(oryginał).
        /// </summary>
        public void ApplyRotation(SceneOrientation orientation)
        {
            for (int i = 0; i < PointsPerSide; i++)
            {
                for (int j = 0; j < PointsPerSide; j++)
                {
                    RotatedControlPoints[i, j] = orientation.RotatePoint(ControlPoints[i, j]);
                }
            }
        }
    }
}
