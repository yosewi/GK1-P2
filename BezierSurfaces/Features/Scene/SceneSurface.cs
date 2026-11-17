using System.Numerics;
using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.Features.Texturing;
using BezierSurfaces.Features.Triangulation;

namespace BezierSurfaces.Features.Scene
{
    // Jedna powierzchnia w scenie razem z jej siatką trójkątów i własnym kolorem obiektu.
    public sealed class SceneSurface
    {
        public BezierSurface Surface { get; }
        public TriangleMesh Mesh { get; private set; }

        // Stały kolor obiektu tej powierzchni.
        public Vector3 ObjectColor { get; set; }

        // Tekstura tej powierzchni.
        public Texture? Texture { get; set; }

        // Czy kolor obiektu tej powierzchni brać z tekstury zamiast stałego koloru.
        public bool UseTexture { get; set; }

        // Tekstura, której trzeba użyć przy rysowaniu, albo null, gdy rysujemy stałym kolorem.
        public Texture? ActiveTexture => UseTexture ? Texture : null;

        // Buduje siatkę o podanej dokładności i ustawia startowy kolor obiektu.
        public SceneSurface(BezierSurface surface, int steps, Vector3 objectColor)
        {
            Surface = surface;
            Mesh = MeshBuilder.Build(surface, steps);
            ObjectColor = objectColor;
        }

        // Buduje siatkę od nowa, np. po zmianie dokładności suwakiem.
        public void RebuildMesh(int steps)
        {
            Mesh = MeshBuilder.Build(Surface, steps);
        }

        // Obraca punkty kontrolne (do wieloboku) i wierzchołki siatki (do siatki i wypełnienia).
        public void ApplyRotation(SceneOrientation orientation)
        {
            Surface.ApplyRotation(orientation);
            Mesh.ApplyRotation(orientation);
        }
    }
}
