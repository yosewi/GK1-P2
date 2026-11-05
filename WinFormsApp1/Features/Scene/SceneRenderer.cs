using System.Drawing.Drawing2D;
using WinFormsApp1.Features.Surfaces;
using WinFormsApp1.Features.Triangulation;
using WinFormsApp1.Rendering;

namespace WinFormsApp1.Features.Scene
{
    /// <summary>
    /// Rysuje jedną klatkę sceny do podanej bitmapy.
    ///
    /// Kolejność rysowania:
    ///  1. tło,
    ///  2. (później) wypełnione trójkąty - prosto do pikseli, z Z-buforem,
    ///  3. linie przez Graphics, zawsze na wierzchu: siatka trójkątów, a na niej wielobok Béziera.
    /// </summary>
    public sealed class SceneRenderer
    {
        private static readonly Color BackgroundColor = Color.White;

        /// <summary>Kolory kolejnych powierzchni (gdy powierzchni jest więcej, kolory się powtarzają).</summary>
        private static readonly Color[] SurfaceColors =
        {
            Color.ForestGreen,
            Color.MediumVioletRed,
            Color.DarkOrange,
            Color.SteelBlue,
        };

        /// <summary>Siatkę rysujemy kolorem powierzchni, ale półprzezroczyście, żeby wielobok był na niej czytelny.</summary>
        private const int MeshAlpha = 110;

        /// <summary>Checkbox "Triangle mesh".</summary>
        public bool ShowTriangleMesh { get; set; } = true;

        /// <summary>Checkbox "Bezier polygon".</summary>
        public bool ShowBezierPolygon { get; set; } = true;

        public void Render(DirectBitmap frame, SceneModel scene)
        {
            ScreenMapper mapper = new ScreenMapper(frame.Width, frame.Height);

            frame.Clear(BackgroundColor);

            using Graphics g = Graphics.FromImage(frame.Bitmap);
            mapper.ApplyWorldTransform(g);
            // Bez wygładzania linii (anti-aliasing): w GDI+ jest bardzo wolne, a linii jest dużo
            // (przy Steps = 60 ponad 40 000 krawędzi trójkątów na klatkę).
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
