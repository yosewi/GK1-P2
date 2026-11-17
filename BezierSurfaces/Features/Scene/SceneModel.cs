using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.Features.Lighting;
using BezierSurfaces.Features.NormalMapping;

namespace BezierSurfaces.Features.Scene
{
    // Stan sceny - wszystkie powierzchnie, ich wspólny obrót i dokładność triangulacji.
    public sealed class SceneModel
    {
        // Dokładność triangulacji na starcie programu.
        public const int DefaultSteps = 10;

        // Startowe kolory obiektu kolejnych powierzchni
        private static readonly Color[] DefaultObjectColors =
        {
            Color.LightSteelBlue,
            Color.LightPink,
            Color.PaleGreen,
            Color.Khaki,
        };

        // Powierzchnie wczytane z pliku, każda razem ze swoją siatką trójkątów.
        public List<SceneSurface> Surfaces { get; } = new List<SceneSurface>();

        // Jeden obrót wspólny dla całej sceny.
        public SceneOrientation Orientation { get; } = new SceneOrientation();

        // Bieżąca dokładność triangulacji, ustawiana suwakiem.
        public int Steps { get; private set; } = DefaultSteps;

        // Współczynniki i kolor światła modelu oświetlenia, wspólne dla wszystkich powierzchni.
        public LightingModel Lighting { get; } = new LightingModel();

        // Źródło światła.
        public LightSource Light { get; } = new LightSource();

        // Wczytana mapa wektorów normalnych.
        public NormalMap? NormalMap { get; set; }

        // Czy zmieniać wektory normalne na podstawie mapy.
        public bool UseNormalMap { get; set; }

        // Mapa normalnych, której trzeba użyć przy rysowaniu, albo null, gdy jest wyłączona.
        public NormalMap? ActiveNormalMap => UseNormalMap ? NormalMap : null;

        // Zastępuje powierzchnie nowymi i od razu buduje dla nich siatki.
        // Kolejne powierzchnie dostają kolejne kolory startowe.
        public void SetSurfaces(IEnumerable<BezierSurface> surfaces)
        {
            Surfaces.Clear();
            foreach (BezierSurface surface in surfaces)
            {
                Color color = DefaultObjectColors[Surfaces.Count % DefaultObjectColors.Length];
                Surfaces.Add(new SceneSurface(surface, Steps, LightingModel.FromColor(color)));
            }
        }

        // Zmienia dokładność i przebudowuje siatki wszystkich powierzchni.
        public void SetSteps(int steps)
        {
            Steps = steps;
            foreach (SceneSurface sceneSurface in Surfaces)
            {
                sceneSurface.RebuildMesh(steps);
            }
        }

        // Obraca wszystkie powierzchnie i siatki bieżącym obrotem. Wołane przed narysowaniem każdej klatki.
        public void ApplyOrientation()
        {
            foreach (SceneSurface sceneSurface in Surfaces)
            {
                sceneSurface.ApplyRotation(Orientation);
            }
        }
    }
}
