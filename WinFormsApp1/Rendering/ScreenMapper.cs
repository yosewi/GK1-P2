namespace WinFormsApp1.Rendering
{
    /// <summary>
    /// Zamiana współrzędnych "świata" na współrzędne pikseli obszaru rysowania.
    ///
    /// Świat:  (0, 0) w środku obszaru rysowania, x w prawo, y w GÓRĘ.
    /// Piksel: (0, 0) w lewym górnym rogu,         x w prawo, y w DÓŁ.
    ///
    /// Wzór:  px = środekX + x
    ///        py = środekY - y     (minus, bo osie y są odwrócone)
    ///
    /// To jedyne miejsce w programie, które zna ten wzór.
    /// </summary>
    public sealed class ScreenMapper
    {
        public float CenterX { get; }
        public float CenterY { get; }

        public ScreenMapper(int width, int height)
        {
            CenterX = width / 2f;
            CenterY = height / 2f;
        }

        /// <summary>
        /// Punkt świata (x, y) -> piksel, którego używamy przy zapisie do DirectBitmap.
        /// </summary>
        public Point ToPixel(float x, float y)
        {
            int px = (int)MathF.Round(CenterX + x);
            int py = (int)MathF.Round(CenterY - y);
            return new Point(px, py);
        }

        /// <summary>
        /// Ustawia na obiekcie Graphics tę samą zamianę współrzędnych,
        /// żeby DrawLine, DrawEllipse itd. przyjmowały współrzędne świata.
        /// To jest kod z treści zadania (ScaleTransform + TranslateTransform).
        /// </summary>
        public void ApplyWorldTransform(Graphics g)
        {
            g.ScaleTransform(1, -1);
            g.TranslateTransform(CenterX, -CenterY);
        }
    }
}
