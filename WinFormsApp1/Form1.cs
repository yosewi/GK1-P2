using WinFormsApp1.Features.Scene;
using WinFormsApp1.Features.Surfaces;
using WinFormsApp1.UI;

namespace WinFormsApp1
{
    /// <summary>
    /// Główne okno. Tylko składa elementy i łączy je ze sobą:
    /// obszar rysowania (RenderCanvas), panel kontrolek (ControlPanel),
    /// scenę (SceneModel) i rysowanie sceny (SceneRenderer). Żadnej logiki tutaj nie piszemy.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>Plik z powierzchniami, kopiowany przy kompilacji obok .exe (do folderu Data).</summary>
        private static readonly string DefaultSurfacesPath =
            Path.Combine(AppContext.BaseDirectory, "Data", "surfaces.txt");

        private readonly RenderCanvas _canvas = new RenderCanvas();
        private readonly ControlPanel _controlPanel = new ControlPanel();
        private readonly SceneModel _scene = new SceneModel();
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

            // Treść zadania: plik należy wczytać podczas uruchamiania programu.
            LoadSurfaces(DefaultSurfacesPath);

            ConnectControls();

            _canvas.FrameRecreated += (sender, e) => Redraw();
            _canvas.RecreateFrame();
        }

        /// <summary>
        /// Łączy kontrolki z panelu z ustawieniami rysowania.
        /// Każda zmiana kontrolki: zapisz nową wartość, narysuj klatkę od nowa.
        /// </summary>
        private void ConnectControls()
        {
            _sceneRenderer.ShowBezierPolygon = _controlPanel.BezierPolygonCheckBox.Checked;
            _controlPanel.BezierPolygonCheckBox.CheckedChanged += (sender, e) =>
            {
                _sceneRenderer.ShowBezierPolygon = _controlPanel.BezierPolygonCheckBox.Checked;
                Redraw();
            };
        }

        /// <summary>
        /// Wczytuje powierzchnie z pliku do sceny.
        /// Przy błędzie pokazuje komunikat, a scena zostaje bez zmian.
        /// </summary>
        private void LoadSurfaces(string path)
        {
            try
            {
                List<BezierSurface> surfaces = SurfaceFileReader.Read(path);
                _scene.SetSurfaces(surfaces);
            }
            catch (Exception ex) when (ex is IOException or FormatException or UnauthorizedAccessException)
            {
                MessageBox.Show($"Could not load surfaces from:\n{path}\n\n{ex.Message}",
                    "Load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Redraw()
        {
            if (_canvas.Frame == null)
            {
                return;
            }

            _sceneRenderer.Render(_canvas.Frame, _scene);
            _canvas.Invalidate();
        }
    }
}
