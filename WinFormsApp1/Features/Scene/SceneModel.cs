using WinFormsApp1.Features.Rotation;
using WinFormsApp1.Features.Surfaces;

namespace WinFormsApp1.Features.Scene
{
    /// <summary>
    /// Wszystko, co jest na scenie: powierzchnie (każda ze swoją siatką trójkątów),
    /// wspólna orientacja (obrót) sceny i dokładność triangulacji.
    /// Później dojdzie tu źródło światła (ono NIE obraca się razem ze sceną - ustalenie z konsultacji).
    ///
    /// (Nazwa SceneModel, a nie Scene, bo "Scene" to już nazwa folderu/przestrzeni nazw.)
    /// </summary>
    public sealed class SceneModel
    {
        /// <summary>Dokładność triangulacji na starcie programu (także początkowa wartość suwaka "Steps").</summary>
        public const int DefaultSteps = 10;

        public List<SceneSurface> Surfaces { get; } = new List<SceneSurface>();

        /// <summary>Jedna orientacja dla wszystkich powierzchni - obracamy całą scenę naraz.</summary>
        public SceneOrientation Orientation { get; } = new SceneOrientation();

        /// <summary>Dokładność triangulacji (suwak "Steps"): na ile odcinków dzielimy u i v.</summary>
        public int Steps { get; private set; } = DefaultSteps;

        /// <summary>Zastępuje wszystkie powierzchnie nowymi (np. po wczytaniu pliku).</summary>
        public void SetSurfaces(IEnumerable<BezierSurface> surfaces)
        {
            Surfaces.Clear();
            foreach (BezierSurface surface in surfaces)
            {
                Surfaces.Add(new SceneSurface(surface, Steps));
            }
        }

        /// <summary>Zmienia dokładność triangulacji i buduje wszystkie siatki od nowa.</summary>
        public void SetSteps(int steps)
        {
            Steps = steps;
            foreach (SceneSurface sceneSurface in Surfaces)
            {
                sceneSurface.RebuildMesh(steps);
            }
        }

        /// <summary>
        /// Przelicza wszystko, co się obraca (punkty kontrolne i wierzchołki siatek),
        /// według aktualnej orientacji. Wołamy raz na klatkę, tuż przed rysowaniem -
        /// dzięki temu nowe siatki i każda zmiana orientacji są zawsze uwzględnione.
        /// </summary>
        public void ApplyOrientation()
        {
            foreach (SceneSurface sceneSurface in Surfaces)
            {
                sceneSurface.ApplyRotation(Orientation);
            }
        }
    }
}
