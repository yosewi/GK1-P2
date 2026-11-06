using BezierSurfaces.Rendering;

namespace BezierSurfaces.UI
{
    public sealed class RenderCanvas : PictureBox
    {
        private bool _redrawRequested;

        public DirectBitmap? Frame { get; private set; }

        public event EventHandler? RenderFrame;

        public RenderCanvas()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            SizeMode = PictureBoxSizeMode.Normal;
        }

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

            base.OnPaint(pe);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecreateFrame();
        }

        private void RecreateFrame()
        {
            int width = Math.Max(1, ClientSize.Width);
            int height = Math.Max(1, ClientSize.Height);

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
