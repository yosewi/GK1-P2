using WinFormsApp1.Rendering;

namespace WinFormsApp1.UI
{
    /// <summary>
    /// Obszar rysowania. Trzyma szybką bitmapę (DirectBitmap) o swoim rozmiarze
    /// i tworzy ją od nowa, gdy zmienia się rozmiar okna.
    ///
    /// Jak działa odświeżanie (żeby nie było lagów):
    ///   - kto chce zmienić obraz (mysz, suwak, checkbox), woła tylko RequestRedraw(),
    ///   - RequestRedraw() zaznacza "trzeba przerysować" i woła Invalidate(),
    ///   - Windows łączy wiele Invalidate() w JEDNO odświeżenie (zdarzenie Paint),
    ///   - dopiero w Paint rysujemy klatkę (zdarzenie RenderFrame) i ją pokazujemy.
    /// Dzięki temu przy szybkim ruchu myszy nie rysujemy klatki przy każdym zdarzeniu myszy,
    /// tylko najwyżej raz na odświeżenie ekranu.
    ///
    /// Sama klasa nic nie rysuje - robi to ten, kto obsługuje zdarzenie RenderFrame.
    /// </summary>
    public sealed class RenderCanvas : PictureBox
    {
        private bool _redrawRequested;

        public DirectBitmap? Frame { get; private set; }

        /// <summary>Zgłaszane, gdy trzeba narysować klatkę do Frame (tuż przed pokazaniem jej na ekranie).</summary>
        public event EventHandler? RenderFrame;

        public RenderCanvas()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            SizeMode = PictureBoxSizeMode.Normal;
        }

        /// <summary>Prosi o narysowanie klatki od nowa przy najbliższym odświeżeniu ekranu.</summary>
        public void RequestRedraw()
        {
            _redrawRequested = true;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (_redrawRequested && Frame != null)
            {
                _redrawRequested = false;
                RenderFrame?.Invoke(this, EventArgs.Empty);
            }

            base.OnPaint(pe); // PictureBox pokazuje bitmapę (Image = Frame.Bitmap)
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecreateFrame();
        }

        private void RecreateFrame()
        {
            // Przy minimalizacji okna rozmiar może spaść do 0, a bitmapa 0x0 nie istnieje.
            int width = Math.Max(1, ClientSize.Width);
            int height = Math.Max(1, ClientSize.Height);

            // Najpierw odpinamy starą bitmapę od PictureBox, dopiero potem ją zwalniamy.
            Image = null;
            Frame?.Dispose();

            Frame = new DirectBitmap(width, height);
            Image = Frame.Bitmap;

            RequestRedraw();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Image = null;
                Frame?.Dispose();
                Frame = null;
            }

            base.Dispose(disposing);
        }
    }
}
