using WinFormsApp1.Features.Rotation;
using WinFormsApp1.Features.Surfaces;
using WinFormsApp1.Features.Triangulation;

namespace WinFormsApp1.Features.Scene
{
    /// <summary>
    /// Jedna powierzchnia na scenie: powierzchnia Béziera (punkty kontrolne)
    /// razem ze swoją siatką trójkątów.
    /// </summary>
    public sealed class SceneSurface
    {
        public BezierSurface Surface { get; }
        public TriangleMesh Mesh { get; private set; }

        public SceneSurface(BezierSurface surface, int steps)
        {
            Surface = surface;
            Mesh = MeshBuilder.Build(surface, steps);
        }

        /// <summary>Buduje siatkę od nowa z inną dokładnością (suwak "Steps").</summary>
        public void RebuildMesh(int steps)
        {
            Mesh = MeshBuilder.Build(Surface, steps);
        }

        /// <summary>Obraca punkty kontrolne i wszystkie wierzchołki siatki.</summary>
        public void ApplyRotation(SceneOrientation orientation)
        {
            Surface.ApplyRotation(orientation);
            Mesh.ApplyRotation(orientation);
        }
    }
}
