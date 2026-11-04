using System.Drawing.Drawing2D;
using WinFormsApp1.Features.Surfaces;
using WinFormsApp1.Rendering;

namespace WinFormsApp1.Features.Scene
{
    /// <summary>
    /// Rysuje jedną klatkę sceny do podanej bitmapy.
    ///
    /// Kolejność rysowania:
    ///  1. tło,
    ///  2. (później) wypełnione trójkąty - prosto do pikseli, z Z-buforem,
    ///  3. linie przez Graphics, zawsze na wierzchu: (później) siatka trójkątów, wielobok Béziera.
    /// </summary>
    public sealed class SceneRenderer
    {
        private static readonly Color BackgroundColor = Color.White;

        /// <summary>Kolory wieloboków kolejnych powierzchni (gdy powierzchni jest więcej, kolory się powtarzają).</summary>
        private static readonly Color[] SurfaceColors =
        {
            Color.ForestGreen,
            Color.MediumVioletRed,
            Color.DarkOrange,
            Color.SteelBlue,
        };

        /// <summary>Checkbox "Bezier polygon".</summary>
        public bool ShowBezierPolygon { get; set; } = true;

        public void Render(DirectBitmap frame, SceneModel scene)
        {
            ScreenMapper mapper = new ScreenMapper(frame.Width, frame.Height);

            frame.Clear(BackgroundColor);

            using Graphics g = Graphics.FromImage(frame.Bitmap);
            mapper.ApplyWorldTransform(g);
            g.SmoothingMode = SmoothingMode.AntiAlias; // gładsze linie

            if (ShowBezierPolygon)
            {
                for (int index = 0; index < scene.Surfaces.Count; index++)
                {
                    Color color = SurfaceColors[index % SurfaceColors.Length];
                    BezierPolygonDrawer.Draw(g, scene.Surfaces[index], color);
                }
            }
        }
    }
}
