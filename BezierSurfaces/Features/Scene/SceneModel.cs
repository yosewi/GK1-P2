using System.Numerics;
using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.Features.Texturing;
using BezierSurfaces.Features.Lighting;

namespace BezierSurfaces.Features.Scene
{
    // Stan sceny - wszystkie powierzchnie, ich wspólny obrót i dokładność triangulacji.
    public sealed class SceneModel
    {
        // Dokładność triangulacji na starcie programu.
        public const int DefaultSteps = 10;

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

        // Kolor obiektu, wspólny dla wszystkich powierzchni
        public Vector3 ObjectColor { get; set; } = LightingModel.FromColor(Color.LightSteelBlue);

        // Wczytana tekstura
        public Texture? Texture { get; set; }

        // Czy kolor obiektu brać z tekstury zamiast stałego koloru.
        public bool UseTexture { get; set; }

        // Tekstura, której trzeba użyć przy rysowaniu, albo null, gdy rysujemy stałym kolorem.
        public Texture? ActiveTexture => UseTexture ? Texture : null;

        // Zastępuje powierzchnie nowymi i od razu buduje dla nich siatki.
        public void SetSurfaces(IEnumerable<BezierSurface> surfaces)
        {
            Surfaces.Clear();
            foreach (BezierSurface surface in surfaces)
            {
                Surfaces.Add(new SceneSurface(surface, Steps));
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
