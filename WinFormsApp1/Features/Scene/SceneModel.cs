using WinFormsApp1.Features.Surfaces;

namespace WinFormsApp1.Features.Scene
{
    /// <summary>
    /// Wszystko, co jest na scenie. Na razie tylko powierzchnie Béziera.
    /// Później dojdą tu: orientacja sceny (obrót) i źródło światła.
    ///
    /// (Nazwa SceneModel, a nie Scene, bo "Scene" to już nazwa folderu/przestrzeni nazw.)
    /// </summary>
    public sealed class SceneModel
    {
        public List<BezierSurface> Surfaces { get; } = new List<BezierSurface>();

        /// <summary>Zastępuje wszystkie powierzchnie nowymi (np. po wczytaniu pliku).</summary>
        public void SetSurfaces(IEnumerable<BezierSurface> surfaces)
        {
            Surfaces.Clear();
            Surfaces.AddRange(surfaces);
        }
    }
}
