using WinFormsApp1.Rendering;

namespace WinFormsApp1.UI
{
    /// <summary>
    /// Obszar rysowania. Trzyma szybką bitmapę (DirectBitmap) o swoim rozmiarze
    /// i tworzy ją od nowa, gdy zmienia się rozmiar okna.
    ///
    /// Sam nic nie rysuje: po utworzeniu nowej bitmapy zgłasza zdarzenie FrameRecreated,
    /// a rysowaniem zajmuje się ktoś inny (na razie SceneRenderer).
    /// </summary>
    public sealed class RenderCanvas : PictureBox
    {
        public DirectBitmap? Frame { get; private set; }

        /// <summary>Zgłaszane po utworzeniu nowej bitmapy, czyli wtedy, gdy trzeba narysować klatkę od nowa.</summary>
        public event EventHandler? FrameRecreated;

        public RenderCanvas()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            SizeMode = PictureBoxSizeMode.Normal;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecreateFrame();
        }

        public void RecreateFrame()
        {
            // Przy minimalizacji okna rozmiar może spaść do 0, a bitmapa 0x0 nie istnieje.
            int width = Math.Max(1, ClientSize.Width);
            int height = Math.Max(1, ClientSize.Height);

            // Najpierw odpinamy starą bitmapę od PictureBox, dopiero potem ją zwalniamy.
            Image = null;
            Frame?.Dispose();

            Frame = new DirectBitmap(width, height);
            Image = Frame.Bitmap;

            FrameRecreated?.Invoke(this, EventArgs.Empty);
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
