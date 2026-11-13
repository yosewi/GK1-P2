using System.Numerics;
using BezierSurfaces.Features.Rotation;
using BezierSurfaces.Features.Scene;
using BezierSurfaces.Features.Surfaces;
using BezierSurfaces.UI;
using BezierSurfaces.Features.Lighting;

namespace BezierSurfaces
{
    public partial class Form1 : Form
    {
        private static readonly string DefaultSurfacesPath = Path.Combine(AppContext.BaseDirectory, "Data", "surfaces.txt");

        private readonly RenderCanvas _canvas = new RenderCanvas();
        private readonly ControlPanel _controlPanel = new ControlPanel();
        private readonly SceneModel _scene = new SceneModel();
        private readonly SceneRenderer _sceneRenderer = new SceneRenderer();
        private MouseRotationController? _mouseRotation;

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
            ConnectObjectColorControls();
            ConnectMouseRotation();

            LoadSurfaces(DefaultSurfacesPath);

            _canvas.RenderFrame += (sender, e) => RenderFrame();
            _canvas.RequestRedraw();
        }

        private void ConnectControls()
        {
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

        private void ConnectObjectColorControls()
        {
            _controlPanel.ChooseColorButton.Click += (sender, e) => ChooseObjectColor();
        }

        private void ChooseLightColor()
        {
            if (TryPickColor(_scene.Lighting.LightColor, out Vector3 color))
            {
                _scene.Lighting.LightColor = color;
                _canvas.RequestRedraw();
            }
        }

        private void ChooseObjectColor()
        {
            if (TryPickColor(_scene.ObjectColor, out Vector3 color))
            {
                _scene.ObjectColor = color;
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
        }

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
