using System.Numerics;
using BezierSurfaces.Features.NormalMapping;
using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Features.Scene;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.Features.Texturing;
using BezierSurfaces.UI;
using BezierSurfaces.Features.Lighting;

namespace BezierSurfaces
{
    public partial class Form1 : Form
    {
        private static readonly string DefaultSurfacesPath = Path.Combine(AppContext.BaseDirectory, "Data", "surfaces.txt");
        private static readonly string DefaultTexturePath = Path.Combine(AppContext.BaseDirectory, "Data", "bricks.png");
        private static readonly string DefaultNormalMapPath = Path.Combine(AppContext.BaseDirectory, "Data", "normal_4.png");

        private readonly RenderCanvas _canvas = new RenderCanvas();
        private readonly ControlPanel _controlPanel = new ControlPanel();
        private readonly SceneModel _scene = new SceneModel();
        private readonly SceneRenderer _sceneRenderer = new SceneRenderer();
        private MouseRotationController? _mouseRotation;
        private LightPathEditor? _lightPathEditor;
        private int _selectedSurfaceIndex;

        private readonly LightAnimator _lightAnimator = new LightAnimator();
        private readonly System.Windows.Forms.Timer _lightTimer = new System.Windows.Forms.Timer { Interval = 30 };

        public Form1()
        {
            InitializeComponent();

            Text = "Bezier Surfaces";
            ClientSize = new Size(1200, 800);
            MinimumSize = new Size(800, 600);

            Controls.Add(_controlPanel);
            Controls.Add(_canvas);
            _canvas.BringToFront();

            _scene.Orientation.SetDefaultView();

            ConnectControls();
            ConnectLightingControls();
            ConnectLightAnimation();
            ConnectLightPathEditing();
            ConnectObjectColorControls();
            ConnectNormalMapControls();
            ConnectMouseRotation();

            LoadSurfaces(DefaultSurfacesPath);
            TryLoadTexture(DefaultTexturePath, _scene.Surfaces);
            TryLoadNormalMap(DefaultNormalMapPath);

            _canvas.RenderFrame += (sender, e) => RenderFrame();
            _canvas.RequestRedraw();
        }

        private void ConnectControls()
        {
            _controlPanel.DocumentationButton.Click += (sender, e) => ShowDocumentation();

            _scene.SetSteps(_controlPanel.StepsTrackBar.Value);
            _controlPanel.StepsTrackBar.ValueChanged += (sender, e) =>
            {
                _scene.SetSteps(_controlPanel.StepsTrackBar.Value);
                _canvas.RequestRedraw();
            };

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

            _sceneRenderer.ShowFilledTriangles = _controlPanel.FilledTrianglesCheckBox.Checked;
            _controlPanel.FilledTrianglesCheckBox.CheckedChanged += (sender, e) =>
            {
                _sceneRenderer.ShowFilledTriangles = _controlPanel.FilledTrianglesCheckBox.Checked;
                _canvas.RequestRedraw();
            };
        }

        private void ConnectLightingControls()
        {
            LightingModel lighting = _scene.Lighting;

            lighting.Kd = ControlPanel.FromHundredths(_controlPanel.KdTrackBar.Value);
            _controlPanel.KdTrackBar.ValueChanged += (sender, e) =>
            {
                lighting.Kd = ControlPanel.FromHundredths(_controlPanel.KdTrackBar.Value);
                _canvas.RequestRedraw();
            };

            lighting.Ks = ControlPanel.FromHundredths(_controlPanel.KsTrackBar.Value);
            _controlPanel.KsTrackBar.ValueChanged += (sender, e) =>
            {
                lighting.Ks = ControlPanel.FromHundredths(_controlPanel.KsTrackBar.Value);
                _canvas.RequestRedraw();
            };

            lighting.M = _controlPanel.MTrackBar.Value;
            _controlPanel.MTrackBar.ValueChanged += (sender, e) =>
            {
                lighting.M = _controlPanel.MTrackBar.Value;
                _canvas.RequestRedraw();
            };

            _scene.Light.SetHeight(_controlPanel.LightZTrackBar.Value);
            _controlPanel.LightZTrackBar.ValueChanged += (sender, e) =>
            {
                _scene.Light.SetHeight(_controlPanel.LightZTrackBar.Value);
                _canvas.RequestRedraw();
            };

            _controlPanel.LightColorButton.Click += (sender, e) => ChooseLightColor();
        }

        private void ConnectLightAnimation()
        {
            _lightTimer.Tick += (sender, e) => _canvas.RequestRedraw();

            SetLightAnimationRunning(_controlPanel.AnimateLightCheckBox.Checked);
            _controlPanel.AnimateLightCheckBox.CheckedChanged += (sender, e) =>
            {
                SetLightAnimationRunning(_controlPanel.AnimateLightCheckBox.Checked);
            };

            _sceneRenderer.ShowLightPathPoints = _controlPanel.ShowLightCurveCheckBox.Checked;
            _controlPanel.ShowLightCurveCheckBox.CheckedChanged += (sender, e) =>
            {
                _sceneRenderer.ShowLightPathPoints = _controlPanel.ShowLightCurveCheckBox.Checked;
                _canvas.RequestRedraw();
            };
        }

        private void ConnectLightPathEditing()
        {
            LightPathEditor editor = new LightPathEditor(_canvas, _scene.Light.Path);
            editor.PathChanged += (sender, e) => _canvas.RequestRedraw();

            editor.IsEnabled = _controlPanel.ShowLightCurveCheckBox.Checked;
            _controlPanel.ShowLightCurveCheckBox.CheckedChanged += (sender, e) =>
            {
                editor.IsEnabled = _controlPanel.ShowLightCurveCheckBox.Checked;
            };

            _lightPathEditor = editor;
        }

        private void SetLightAnimationRunning(bool running)
        {
            _lightAnimator.IsRunning = running;
            _lightTimer.Enabled = running;
        }

        private void ConnectObjectColorControls()
        {
            _controlPanel.SurfaceComboBox.SelectedIndexChanged += (sender, e) =>
            {
                SelectSurface(_controlPanel.SurfaceComboBox.SelectedIndex);
            };

            _controlPanel.ChooseColorButton.Click += (sender, e) => ChooseObjectColor();

            _controlPanel.TextureRadioButton.CheckedChanged += (sender, e) =>
            {
                SceneSurface? surface = SelectedSurface;
                if (surface != null)
                {
                    surface.UseTexture = _controlPanel.TextureRadioButton.Checked;
                    _canvas.RequestRedraw();
                }
            };

            _controlPanel.LoadTextureButton.Click += (sender, e) => LoadTexture();
        }

        private void ConnectNormalMapControls()
        {
            _scene.UseNormalMap = _controlPanel.UseNormalMapCheckBox.Checked;
            _controlPanel.UseNormalMapCheckBox.CheckedChanged += (sender, e) =>
            {
                _scene.UseNormalMap = _controlPanel.UseNormalMapCheckBox.Checked;
                _canvas.RequestRedraw();
            };

            _controlPanel.LoadNormalMapButton.Click += (sender, e) => LoadNormalMap();
        }

        private void ChooseLightColor()
        {
            if (TryPickColor(_scene.Lighting.LightColor, out Vector3 color))
            {
                _scene.Lighting.LightColor = color;
                _canvas.RequestRedraw();
            }
        }

        private SceneSurface? SelectedSurface =>
            _selectedSurfaceIndex < _scene.Surfaces.Count ? _scene.Surfaces[_selectedSurfaceIndex] : null;

        private void SelectSurface(int index)
        {
            _selectedSurfaceIndex = Math.Max(0, index);

            SceneSurface? surface = SelectedSurface;
            if (surface == null)
            {
                return;
            }

            if (surface.UseTexture)
            {
                _controlPanel.TextureRadioButton.Checked = true;
            }
            else
            {
                _controlPanel.ConstantColorRadioButton.Checked = true;
            }
        }

        private void UpdateSurfaceList()
        {
            ComboBox list = _controlPanel.SurfaceComboBox;
            list.Items.Clear();

            for (int index = 0; index < _scene.Surfaces.Count; index++)
            {
                list.Items.Add($"Surface {index + 1}");
            }

            if (list.Items.Count > 0)
            {
                list.SelectedIndex = 0;
            }
        }

        private void ChooseObjectColor()
        {
            SceneSurface? surface = SelectedSurface;
            if (surface != null && TryPickColor(surface.ObjectColor, out Vector3 color))
            {
                surface.ObjectColor = color;
                _canvas.RequestRedraw();
            }
        }

        private bool TryPickColor(Vector3 current, out Vector3 chosen)
        {
            using ColorDialog dialog = new ColorDialog { Color = LightingModel.ToColor(current) };

            bool accepted = dialog.ShowDialog(this) == DialogResult.OK;
            chosen = accepted ? LightingModel.FromColor(dialog.Color) : current;
            return accepted;
        }

        private void ConnectMouseRotation()
        {
            _mouseRotation = new MouseRotationController(_canvas, _scene.Orientation);
            _mouseRotation.OrientationChanged += (sender, e) => _canvas.RequestRedraw();
            _mouseRotation.ShouldIgnoreDragAt = location => _lightPathEditor?.CanGrabAt(location) == true;
        }

        private void LoadTexture()
        {
            SceneSurface? surface = SelectedSurface;
            if (surface == null)
            {
                return;
            }

            string? path = AskForImagePath("Load texture");
            if (path != null && TryLoadTexture(path, new[] { surface }))
            {
                _controlPanel.TextureRadioButton.Checked = true;
                _canvas.RequestRedraw();
            }
        }

        private void LoadNormalMap()
        {
            string? path = AskForImagePath("Load normal map");
            if (path != null && TryLoadNormalMap(path))
            {
                _controlPanel.UseNormalMapCheckBox.Checked = true;
                _canvas.RequestRedraw();
            }
        }

        private string? AskForImagePath(string title)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = title,
                Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp|All files|*.*",
            };

            return dialog.ShowDialog(this) == DialogResult.OK ? dialog.FileName : null;
        }

        private bool TryLoadTexture(string path, IEnumerable<SceneSurface> surfaces)
        {
            return TryLoadImage(path, "texture", () =>
            {
                Texture texture = Texture.Load(path);
                foreach (SceneSurface surface in surfaces)
                {
                    surface.Texture = texture;
                }
            });
        }

        private bool TryLoadNormalMap(string path)
        {
            return TryLoadImage(path, "normal map", () => _scene.NormalMap = NormalMap.Load(path));
        }

        private bool TryLoadImage(string path, string description, Action load)
        {
            try
            {
                load();
                return true;
            }
            catch (Exception ex) when (ex is IOException or ArgumentException or OutOfMemoryException)
            {
                MessageBox.Show($"Could not load {description} from:\n{path}\n\n{ex.Message}",
                    "Load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void LoadSurfaces(string path)
        {
            try
            {
                List<BezierSurface> surfaces = SurfaceFileReader.Read(path);
                _scene.SetSurfaces(surfaces);
                UpdateSurfaceList();
            }
            catch (Exception ex) when (ex is IOException or FormatException or UnauthorizedAccessException)
            {
                MessageBox.Show($"Could not load surfaces from:\n{path}\n\n{ex.Message}",
                    "Load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Skróty klawiaturowe. ProcessCmdKey dostaje klawisz przed kontrolkami panelu,
        // więc np. spacja nie przełącza zaznaczonego checkboxa, tylko animację.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F1:
                    ShowDocumentation();
                    return true;

                case Keys.Space:
                    _controlPanel.AnimateLightCheckBox.Checked = !_controlPanel.AnimateLightCheckBox.Checked;
                    return true;

                case Keys.R:
                    _scene.Orientation.SetDefaultView();
                    _canvas.RequestRedraw();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ShowDocumentation()
        {
            string text =
                "Obsługa: przeciąganie lewym przyciskiem myszy obraca scenę (poziomo wokół osi y ekranu, " +
                "pionowo wokół osi x ekranu), kółko myszy obraca wokół osi z. Lewy przycisk na punkcie " +
                "kontrolnym krzywej światła przeciąga ten punkt (gdy włączone jest \"Show light curve points\").\n\n" +
                "Skróty: F1 – dokumentacja, Spacja – zatrzymanie/wznowienie animacji światła, " +
                "R – powrót do widoku startowego.\n\n" +
                "Obrót myszą: orientacja to jedna macierz R. Przy każdym ruchu myszy przesunięcie (dx, dy) " +
                "od poprzedniego zdarzenia daje obroty Ry(0.01·dx) i Rx(0.01·dy), które składamy jako " +
                "R = Rprzyrost · R. Mnożenie z lewej oznacza obrót wokół osi ekranu, więc przednia część sceny " +
                "zawsze podąża za kursorem. Po każdym kroku R jest ortonormalizowana, a punkty i wektory " +
                "obracamy zawsze z wartości oryginalnych.\n\n" +
                "Rysowanie: siatka trójkątów z podziału kwadratu (u, v), wypełnianie scan-line z sortowaniem " +
                "wierzchołków, interpolacja barycentryczna z, N i (u, v), Z-bufor, oświetlenie Lamberta " +
                "ze składową zwierciadlaną dla obu stron powierzchni.\n\n" +
                "Światło porusza się po zamkniętej krzywej Béziera (V6 = V0) na wysokości z. Kolor obiektu " +
                "(stały albo tekstura) jest osobny dla każdej powierzchni, a mapa normalnych zmienia N " +
                "macierzą [Pu, Pv, N].\n\n" +
                "Więcej szczegółów w README i w komentarzach w kodzie.";

            MessageBox.Show(this, text, "Dokumentacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RenderFrame()
        {
            if (_canvas.Frame == null)
            {
                return;
            }

            _lightAnimator.Update(_scene.Light);
            _scene.ApplyOrientation();
            _sceneRenderer.Render(_canvas.Frame, _scene);
        }
    }
}
