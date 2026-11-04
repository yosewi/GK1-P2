using WinFormsApp1.Features.Rotation;
using WinFormsApp1.Features.Surfaces;

namespace WinFormsApp1.Features.Scene
{
    /// <summary>
    /// Wszystko, co jest na scenie: powierzchnie Béziera i wspólna orientacja (obrót) sceny.
    /// Później dojdzie tu źródło światła (ono NIE obraca się razem ze sceną - ustalenie z konsultacji).
    ///
    /// (Nazwa SceneModel, a nie Scene, bo "Scene" to już nazwa folderu/przestrzeni nazw.)
    /// </summary>
    public sealed class SceneModel
    {
        public List<BezierSurface> Surfaces { get; } = new List<BezierSurface>();

        /// <summary>Jedna orientacja dla wszystkich powierzchni - obracamy całą scenę naraz.</summary>
        public SceneOrientation Orientation { get; } = new SceneOrientation();

        /// <summary>Zastępuje wszystkie powierzchnie nowymi (np. po wczytaniu pliku).</summary>
        public void SetSurfaces(IEnumerable<BezierSurface> surfaces)
        {
            Surfaces.Clear();
            Surfaces.AddRange(surfaces);
            ApplyOrientation();
        }

        /// <summary>
        /// Przelicza wszystko, co się obraca, według aktualnej orientacji.
        /// Wołamy po każdej zmianie orientacji (np. po ruchu myszą).
        /// </summary>
        public void ApplyOrientation()
        {
            foreach (BezierSurface surface in Surfaces)
            {
                surface.ApplyRotation(Orientation);
            }
        }
    }
}
