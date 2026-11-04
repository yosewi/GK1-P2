using WinFormsApp1.Features.Scene;
using WinFormsApp1.UI;

namespace WinFormsApp1
{
    /// <summary>
    /// Główne okno. Tylko składa elementy i łączy je ze sobą:
    /// obszar rysowania (RenderCanvas), panel kontrolek (ControlPanel)
    /// i rysowanie sceny (SceneRenderer). Żadnej logiki tutaj nie piszemy.
    /// </summary>
    public partial class Form1 : Form
    {
        private readonly RenderCanvas _canvas = new RenderCanvas();
        private readonly ControlPanel _controlPanel = new ControlPanel();
        private readonly SceneRenderer _sceneRenderer = new SceneRenderer();

        public Form1()
        {
            InitializeComponent();

            Text = "Bezier Surfaces - Triangles & Lighting";
            ClientSize = new Size(1200, 800);
            MinimumSize = new Size(800, 600);

            // Kolejność ma znaczenie przy dokowaniu:
            // panel dokujemy po prawej, a obszar rysowania zajmuje to, co zostanie.
            Controls.Add(_controlPanel);
            Controls.Add(_canvas);
            _canvas.BringToFront();

            _canvas.FrameRecreated += (sender, e) => Redraw();
            _canvas.RecreateFrame();
        }

        private void Redraw()
        {
            if (_canvas.Frame == null)
            {
                return;
            }

            _sceneRenderer.Render(_canvas.Frame);
            _canvas.Invalidate();
        }
    }
}
