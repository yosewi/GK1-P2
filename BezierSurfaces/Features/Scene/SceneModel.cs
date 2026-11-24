using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.Features.Lighting;
using BezierSurfaces.Features.NormalMapping;

namespace BezierSurfaces.Features.Scene
{
    // Stan sceny - wszystkie powierzchnie, ich wspólny obrót i dokładność triangulacji.
    public sealed class SceneModel
    {
        public const int DefaultSteps = 10;

        private static readonly Color[] DefaultObjectColors =
        {
            Color.LightSteelBlue,
            Color.LightPink,
            Color.PaleGreen,
            Color.Khaki,
        };

        public List<SceneSurface> Surfaces { get; } = new List<SceneSurface>();

        public SceneOrientation Orientation { get; } = new SceneOrientation();

        public int Steps { get; private set; } = DefaultSteps;

        // Współczynniki i kolor światła modelu oświetlenia, wspólne dla wszystkich powierzchni.
        public LightingModel Lighting { get; } = new LightingModel();

        public LightSource Light { get; } = new LightSource();

        public NormalMap? NormalMap { get; set; }

        public bool UseNormalMap { get; set; }

        public NormalMap? ActiveNormalMap => UseNormalMap ? NormalMap : null;

        // Zastępuje powierzchnie nowymi i od razu buduje dla nich siatki.
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

        public void ApplyOrientation()
        {
            foreach (SceneSurface sceneSurface in Surfaces)
            {
                sceneSurface.ApplyRotation(Orientation);
            }
        }
    }
}
