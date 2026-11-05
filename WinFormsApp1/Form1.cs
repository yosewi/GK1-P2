using WinFormsApp1.Features.Rotation;
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
        private MouseRotationController? _mouseRotation;

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

            // Widok startowy z ukosa (dalej będziemy go zmieniać myszą).
            _scene.Orientation.SetDefaultView();

            // Najpierw podłączamy kontrolki (m.in. ustawiają dokładność triangulacji z suwaka),
            // dopiero potem wczytujemy plik - wtedy siatki od razu powstają z właściwą dokładnością.
            ConnectControls();
            ConnectMouseRotation();

            // Treść zadania: plik należy wczytać podczas uruchamiania programu.
            // (SetSurfaces od razu buduje siatki i obraca wszystko według aktualnej orientacji.)
            LoadSurfaces(DefaultSurfacesPath);

            // Klatkę rysujemy tylko wtedy, gdy obszar rysowania o to poprosi (patrz RenderCanvas).
            _canvas.RenderFrame += (sender, e) => RenderFrame();
            _canvas.RequestRedraw();
        }

        /// <summary>
        /// Łączy kontrolki z panelu z ustawieniami rysowania.
        /// Każda zmiana kontrolki: zapisz nową wartość i poproś o przerysowanie.
        /// </summary>
        private void ConnectControls()
        {
            // --- Triangulation: suwak "Steps" ---
            _scene.SetSteps(_controlPanel.StepsTrackBar.Value);
            _controlPanel.StepsTrackBar.ValueChanged += (sender, e) =>
            {
                _scene.SetSteps(_controlPanel.StepsTrackBar.Value);
                _canvas.RequestRedraw();
            };

            // --- Display: checkboxy ---
            _sceneRenderer.ShowBezierPolygon = _controlPanel.BezierPolygonCheckBox.Checked;
            _controlPanel.BezierPolygonCheckBox.CheckedChanged += (sender, e) =>
            {
                _sceneRenderer.ShowBezierPolygon = _controlPanel.BezierPolygonCheckBox.Checked;
                _canvas.RequestRedraw();
            };

            _sceneRenderer.ShowTriangleMesh = _controlPanel.TriangleMeshCheckBox.Checked;
            _controlPanel.TriangleMeshCheckBox.CheckedChanged += (sender, e) =>
            {
                _sceneRenderer.ShowTriangleMesh = _controlPanel.TriangleMeshCheckBox.Checked;
                _canvas.RequestRedraw();
            };
        }

        /// <summary>
        /// Przeciąganie myszą i scroll po obszarze rysowania obracają scenę.
        /// Ruch myszy zmienia tylko macierz orientacji (to szybkie) i prosi o przerysowanie.
        /// Wierzchołki obracamy dopiero przy rysowaniu klatki - raz na klatkę, a nie przy każdym ruchu myszy.
        /// </summary>
        private void ConnectMouseRotation()
        {
            _mouseRotation = new MouseRotationController(_canvas, _scene.Orientation);
            _mouseRotation.OrientationChanged += (sender, e) => _canvas.RequestRedraw();
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

        /// <summary>
        /// Rysuje jedną klatkę: obraca scenę według aktualnej orientacji i rysuje ją do bitmapy.
        /// Wołane przez RenderCanvas tuż przed pokazaniem klatki na ekranie.
        /// </summary>
        private void RenderFrame()
        {
            if (_canvas.Frame == null)
            {
                return;
            }

            _scene.ApplyOrientation();
            _sceneRenderer.Render(_canvas.Frame, _scene);
        }
    }
}
