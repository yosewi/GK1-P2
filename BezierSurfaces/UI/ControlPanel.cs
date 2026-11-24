using BezierSurfaces.Features.Scene;
using BezierSurfaces.Features.Triangulation;
using BezierSurfaces.Features.Lighting;

namespace BezierSurfaces.UI
{
    public sealed class ControlPanel : FlowLayoutPanel
    {
        public const int PanelWidth = 260;
        private const int ItemWidth = PanelWidth - 40;

        private const int MaxSteps = 60;

        private const int MinM = 1;
        private const int MaxM = 100;
        private const int MaxLightHeight = 1000;

        public Button DocumentationButton { get; }

        public TrackBar StepsTrackBar { get; }

        public CheckBox BezierPolygonCheckBox { get; }
        public CheckBox TriangleMeshCheckBox { get; }
        public CheckBox FilledTrianglesCheckBox { get; }

        public TrackBar KdTrackBar { get; }
        public TrackBar KsTrackBar { get; }
        public TrackBar MTrackBar { get; }
        public Button LightColorButton { get; }

        public TrackBar LightZTrackBar { get; }
        public CheckBox AnimateLightCheckBox { get; }
        public CheckBox ShowLightCurveCheckBox { get; }

        public ComboBox SurfaceComboBox { get; }
        public RadioButton ConstantColorRadioButton { get; }
        public Button ChooseColorButton { get; }
        public RadioButton TextureRadioButton { get; }
        public Button LoadTextureButton { get; }

        public CheckBox UseNormalMapCheckBox { get; }
        public Button LoadNormalMapButton { get; }

        public ControlPanel()
        {
            Dock = DockStyle.Right;
            Width = PanelWidth;
            FlowDirection = FlowDirection.TopDown;
            WrapContents = false;
            AutoScroll = true;
            Padding = new Padding(8);
            BackColor = SystemColors.Control;

            DocumentationButton = ControlFactory.AddButton(this, "Documentation (F1)", ItemWidth);

            ControlFactory.AddHeader(this, "Triangulation");
            StepsTrackBar = ControlFactory.AddTrackBar(this, "Steps",
                MeshBuilder.MinSteps, MaxSteps, SceneModel.DefaultSteps, ItemWidth, FormatInteger);

            ControlFactory.AddHeader(this, "Display");
            BezierPolygonCheckBox = ControlFactory.AddCheckBox(this, "Bezier polygon", true);
            TriangleMeshCheckBox = ControlFactory.AddCheckBox(this, "Triangle mesh", true);
            FilledTrianglesCheckBox = ControlFactory.AddCheckBox(this, "Filled triangles", false);

            ControlFactory.AddHeader(this, "Lighting");
            KdTrackBar = ControlFactory.AddTrackBar(this, "kd", 0, 100,
                ToHundredths(LightingModel.DefaultKd), ItemWidth, FormatHundredths);
            KsTrackBar = ControlFactory.AddTrackBar(this, "ks", 0, 100,
                ToHundredths(LightingModel.DefaultKs), ItemWidth, FormatHundredths);
            MTrackBar = ControlFactory.AddTrackBar(this, "m", MinM, MaxM,
                LightingModel.DefaultM, ItemWidth, FormatInteger);
            LightColorButton = ControlFactory.AddButton(this, "Light color...", ItemWidth);

            ControlFactory.AddHeader(this, "Light source");
            LightZTrackBar = ControlFactory.AddTrackBar(this, "Light Z", 0, MaxLightHeight,
                LightSource.DefaultHeight, ItemWidth, FormatInteger);
            AnimateLightCheckBox = ControlFactory.AddCheckBox(this, "Animate light", true);
            ShowLightCurveCheckBox = ControlFactory.AddCheckBox(this, "Show light curve points", true);

            ControlFactory.AddHeader(this, "Object color");
            SurfaceComboBox = ControlFactory.AddComboBox(this, ItemWidth);
            ConstantColorRadioButton = ControlFactory.AddRadioButton(this, "Constant color", true);
            ChooseColorButton = ControlFactory.AddButton(this, "Choose color...", ItemWidth);
            TextureRadioButton = ControlFactory.AddRadioButton(this, "Texture", false);
            LoadTextureButton = ControlFactory.AddButton(this, "Load texture...", ItemWidth);

            ControlFactory.AddHeader(this, "Normal map");
            UseNormalMapCheckBox = ControlFactory.AddCheckBox(this, "Use normal map", false);
            LoadNormalMapButton = ControlFactory.AddButton(this, "Load normal map...", ItemWidth);
        }

        public static float FromHundredths(int value) => value / 100f;

        private static int ToHundredths(float value) => (int)MathF.Round(value * 100);

        private static string FormatInteger(int value) => value.ToString();

        private static string FormatHundredths(int value) =>
            (value / 100f).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
    }
}
