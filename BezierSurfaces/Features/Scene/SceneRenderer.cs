using System.Drawing.Drawing2D;
using BezierSurfaces.Features.Filling;
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

        private static readonly Color[] SurfaceFillColors =
        {
            Color.LightGreen,
            Color.Plum,
            Color.NavajoWhite,
            Color.LightSteelBlue,
        };

        private const int MeshAlpha = 110;

        public bool ShowFilledTriangles { get; set; }

        public bool ShowTriangleMesh { get; set; } = true;

        public bool ShowBezierPolygon { get; set; } = true;

        private DepthBuffer? _depthBuffer;

        public void Render(DirectBitmap frame, SceneModel scene)
        {
            ScreenMapper mapper = new ScreenMapper(frame.Width, frame.Height);

            frame.Clear(BackgroundColor);

            if (ShowFilledTriangles)
            {
                DrawFilledTriangles(frame, mapper, scene);
            }

            DrawLines(frame, mapper, scene);
        }

        private void DrawFilledTriangles(DirectBitmap frame, ScreenMapper mapper, SceneModel scene)
        {
            DepthBuffer depthBuffer = GetClearedDepthBuffer(frame.Width, frame.Height);

            for (int index = 0; index < scene.Surfaces.Count; index++)
            {
                Color fillColor = SurfaceFillColors[index % SurfaceFillColors.Length];
                SolidColorMeshFiller.Fill(frame, depthBuffer, mapper, scene.Surfaces[index].Mesh, fillColor);
            }
        }

        private DepthBuffer GetClearedDepthBuffer(int width, int height)
        {
            if (_depthBuffer == null || _depthBuffer.Width != width || _depthBuffer.Height != height)
            {
                _depthBuffer = new DepthBuffer(width, height);
                return _depthBuffer;
            }

            _depthBuffer.Clear();
            return _depthBuffer;
        }

        private void DrawLines(DirectBitmap frame, ScreenMapper mapper, SceneModel scene)
        {
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
