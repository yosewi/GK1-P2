using WinFormsApp1.Rendering;

namespace WinFormsApp1.Features.Scene
{
    /// <summary>
    /// Rysuje jedną klatkę sceny do podanej bitmapy.
    /// Tu w kolejnych krokach dojdzie: wielobok Béziera, siatka trójkątów, wypełnianie, światło.
    /// </summary>
    public sealed class SceneRenderer
    {
        private static readonly Color BackgroundColor = Color.White;

        public void Render(DirectBitmap frame)
        {
            ScreenMapper mapper = new ScreenMapper(frame.Width, frame.Height);

            frame.Clear(BackgroundColor);

            DrawCoordinateSystemTest(frame, mapper);
        }

        /// <summary>
        /// TYMCZASOWY test układu współrzędnych (usuniemy go, gdy pojawią się powierzchnie):
        ///  - osie x i y (przez Graphics, we współrzędnych świata),
        ///  - czerwona kropka w punkcie (100, 50) wpisana bezpośrednio w piksele,
        ///  - niebieski okrąg wokół (100, 50) narysowany przez Graphics.
        /// Kropka w środku okręgu = oba sposoby rysowania liczą współrzędne tak samo.
        /// </summary>
        private static void DrawCoordinateSystemTest(DirectBitmap frame, ScreenMapper mapper)
        {
            Point dot = mapper.ToPixel(100, 50);
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    frame.SetPixel(dot.X + dx, dot.Y + dy, Color.Red);
                }
            }

            using Graphics g = Graphics.FromImage(frame.Bitmap);
            mapper.ApplyWorldTransform(g);

            float halfWidth = frame.Width / 2f;
            float halfHeight = frame.Height / 2f;

            using Pen axisPen = new Pen(Color.Gray);
            g.DrawLine(axisPen, -halfWidth, 0, halfWidth, 0);   // oś x
            g.DrawLine(axisPen, 0, -halfHeight, 0, halfHeight); // oś y

            using Pen circlePen = new Pen(Color.Blue);
            g.DrawEllipse(circlePen, 100 - 8, 50 - 8, 16, 16);
        }
    }
}
