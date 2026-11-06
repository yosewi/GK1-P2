using System.Drawing.Drawing2D;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.Features.Triangulation;
using BezierSurfaces.Rendering;

namespace BezierSurfaces.Features.Scene
{
    public sealed class SceneRenderer
    {
        private static readonly Color BackgroundColor = Color.White;

        private static readonly Color[] SurfaceColors =
        {
            Color.ForestGreen,
            Color.MediumVioletRed,
            Color.DarkOrange,
            Color.SteelBlue,
        };

        private const int MeshAlpha = 110;

        public bool ShowTriangleMesh { get; set; } = true;

        public bool ShowBezierPolygon { get; set; } = true;

        public void Render(DirectBitmap frame, SceneModel scene)
        {
            ScreenMapper mapper = new ScreenMapper(frame.Width, frame.Height);

            frame.Clear(BackgroundColor);

            using Graphics g = Graphics.FromImage(frame.Bitmap);
            mapper.ApplyWorldTransform(g);

            g.SmoothingMode = SmoothingMode.None;

            for (int index = 0; index < scene.Surfaces.Count; index++)
            {
                SceneSurface sceneSurface = scene.Surfaces[index];
                Color color = SurfaceColors[index % SurfaceColors.Length];

                if (ShowTriangleMesh)
                {
                    TriangleMeshDrawer.Draw(g, sceneSurface.Mesh, Color.FromArgb(MeshAlpha, color));
                }

                if (ShowBezierPolygon)
                {
                    BezierPolygonDrawer.Draw(g, sceneSurface.Surface, color);
                }
            }
        }
    }
}
