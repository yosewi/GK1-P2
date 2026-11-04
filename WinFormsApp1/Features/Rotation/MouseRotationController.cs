namespace WinFormsApp1.Features.Rotation
{
    /// <summary>
    /// Zamienia przeciąganie myszą (lewy przycisk) na obrót sceny.
    ///
    /// Obrót jest NARASTAJĄCY: przy każdym ruchu myszy bierzemy tylko przesunięcie od
    /// POPRZEDNIEGO położenia kursora (dx, dy) i doklejamy mały obrót do aktualnej orientacji.
    /// Nie liczymy orientacji od miejsca, w którym zaczęło się przeciąganie (wymaganie z treści).
    ///
    /// Jak ruch myszy zamienia się na obrót (obracamy wokół osi EKRANU):
    ///   - ruch w poziomie (dx) -> obrót wokół pionowej osi ekranu (y),
    ///   - ruch w pionie   (dy) -> obrót wokół poziomej osi ekranu (x).
    /// Kąt jest proporcjonalny do przesunięcia: kąt = dx · czułość (lub dy · czułość).
    /// Znaki są dobrane tak, żeby przednia część sceny (bliżej nas) "jechała" za kursorem,
    /// jakbyśmy chwycili kulę i ją obracali.
    /// </summary>
    public sealed class MouseRotationController
    {
        /// <summary>Ile radianów obrotu na jeden piksel ruchu myszy (0.01 rad ≈ 0.57°).</summary>
        private const float RadiansPerPixel = 0.01f;

        private readonly SceneOrientation _orientation;
        private bool _isDragging;
        private Point _lastMousePosition;

        /// <summary>Zgłaszane po każdej zmianie orientacji, żeby można było przerysować scenę.</summary>
        public event EventHandler? OrientationChanged;

        public MouseRotationController(Control control, SceneOrientation orientation)
        {
            _orientation = orientation;

            control.MouseDown += OnMouseDown;
            control.MouseMove += OnMouseMove;
            control.MouseUp += OnMouseUp;
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            _isDragging = true;
            _lastMousePosition = e.Location;
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDragging)
            {
                return;
            }

            // Przesunięcie od POPRZEDNIEGO zdarzenia (w pikselach ekranu: x w prawo, y w DÓŁ).
            int dx = e.X - _lastMousePosition.X;
            int dy = e.Y - _lastMousePosition.Y;
            _lastMousePosition = e.Location;

            if (dx == 0 && dy == 0)
            {
                return;
            }

            // Ruch w prawo (dx > 0): obrót wokół osi y o kąt dodatni - przód sceny jedzie w prawo.
            // Ruch w dół (dy > 0, bo na ekranie y rośnie w dół): obrót wokół osi x o kąt dodatni -
            // przód sceny jedzie w dół.
            _orientation.RotateAroundScreenY(dx * RadiansPerPixel);
            _orientation.RotateAroundScreenX(dy * RadiansPerPixel);

            OrientationChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
            }
        }
    }
}
