using System.Drawing.Drawing2D;
using FastBitmapLib;
using BezierSurfaces.Features.Filling;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.Features.Triangulation;
using BezierSurfaces.Rendering;

namespace BezierSurfaces.Features.Scene
{
    // Rysuje całą klatkę: tło, wypełnione trójkąty z Z-buforem, a na wierzchu siatkę i wielobok Béziera.
    public sealed class SceneRenderer
    {
        private static readonly Color BackgroundColor = Color.White;

        // Kolory linii kolejnych powierzchni
        private static readonly Color[] SurfaceColors =
        {
            Color.ForestGreen,
            Color.MediumVioletRed,
            Color.DarkOrange,
            Color.SteelBlue,
        };

        // Kolory wypełnienia kolejnych powierzchni.
        private static readonly Color[] SurfaceFillColors =
        {
            Color.LightGreen,
            Color.Plum,
            Color.NavajoWhite,
            Color.LightSteelBlue,
        };

        // Przezroczystość linii siatki, żeby nie zasłaniały wypełnienia.
        private const int MeshAlpha = 110;

        // Co rysować - ustawiane checkboxami w panelu.
        public bool ShowFilledTriangles { get; set; }

        public bool ShowTriangleMesh { get; set; } = true;

        public bool ShowBezierPolygon { get; set; } = true;

        // Z-bufor
        private DepthBuffer? _depthBuffer;

        // Rysuje jedną klatkę na podanej bitmapie.
        public void Render(Bitmap frame, SceneModel scene)
        {
            int width = frame.Width;
            int height = frame.Height;
            ScreenMapper mapper = new ScreenMapper(width, height);

            // Blokujemy bitmapę, żeby szybko pisać piksele
            // Bitmapa odblokowuje się na końcu bloku using.
            using (FastBitmap pixels = frame.FastLock())
            {
                pixels.Clear(BackgroundColor);

                if (ShowFilledTriangles)
                {
                    DepthBuffer depthBuffer = GetClearedDepthBuffer(width, height);
                    DrawFilledTriangles(pixels, depthBuffer, mapper, scene);
                }
            }

            // Linie rysujemy dopiero po odblokowaniu bitmapy
            DrawLines(frame, mapper, scene);
        }

        // Wypełnia trójkąty wszystkich powierzchni.
        private static void DrawFilledTriangles(FastBitmap pixels, DepthBuffer depthBuffer, ScreenMapper mapper, SceneModel scene)
        {
            for (int index = 0; index < scene.Surfaces.Count; index++)
            {
                Color fillColor = SurfaceFillColors[index % SurfaceFillColors.Length];
                SolidColorMeshFiller.Fill(pixels, depthBuffer, mapper, scene.Surfaces[index].Mesh, fillColor);
            }
        }

        // Zwraca wyczyszczony Z-bufor. Nowy tworzy jak zmienił się rozmiar okna.
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

        // Rysuje linie: siatkę trójkątów i wielobok Béziera.
        private void DrawLines(Bitmap frame, ScreenMapper mapper, SceneModel scene)
        {
            using Graphics g = Graphics.FromImage(frame);

            // Ustawia przejście ze współrzędnych sceny na piksele ekranu.
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
