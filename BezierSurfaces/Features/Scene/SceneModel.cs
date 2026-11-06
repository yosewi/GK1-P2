using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Features.Surfaces;

namespace BezierSurfaces.Features.Scene
{
    public sealed class SceneModel
    {
        public const int DefaultSteps = 10;

        public List<SceneSurface> Surfaces { get; } = new List<SceneSurface>();

        public SceneOrientation Orientation { get; } = new SceneOrientation();

        public int Steps { get; private set; } = DefaultSteps;

        public void SetSurfaces(IEnumerable<BezierSurface> surfaces)
        {
            Surfaces.Clear();
            foreach (BezierSurface surface in surfaces)
            {
                Surfaces.Add(new SceneSurface(surface, Steps));
            }
        }

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
