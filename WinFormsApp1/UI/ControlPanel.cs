using WinFormsApp1.Features.Scene;
using WinFormsApp1.Features.Triangulation;

namespace WinFormsApp1.UI
{
    /// <summary>
    /// Panel z kontrolkami po prawej stronie okna.
    /// Zawiera tylko to, czego wymaga treść zadania (układ podobny do programu prowadzącego).
    ///
    /// Panel sam niczego nie liczy i nie rysuje. Udostępnia kontrolki,
    /// a Form1 podłącza je do odpowiednich funkcjonalności.
    /// </summary>
    public sealed class ControlPanel : FlowLayoutPanel
    {
        public const int PanelWidth = 260;
        private const int ItemWidth = PanelWidth - 40;

        /// <summary>Największa dokładność triangulacji na suwaku (najmniejsza to MeshBuilder.MinSteps).</summary>
        private const int MaxSteps = 60;

        // --- Triangulation ---
        public TrackBar StepsTrackBar { get; }

        // --- Display ---
        public CheckBox BezierPolygonCheckBox { get; }
        public CheckBox TriangleMeshCheckBox { get; }
        public CheckBox FilledTrianglesCheckBox { get; }

        // --- Lighting ---
        public TrackBar KdTrackBar { get; }
        public TrackBar KsTrackBar { get; }
        public TrackBar MTrackBar { get; }
        public Button LightColorButton { get; }

        // --- Light source ---
        public TrackBar LightZTrackBar { get; }
        public CheckBox AnimateLightCheckBox { get; }
        public CheckBox ShowLightCurveCheckBox { get; }

        // --- Object color ---
        public RadioButton ConstantColorRadioButton { get; }
        public Button ChooseColorButton { get; }
        public RadioButton TextureRadioButton { get; }
        public Button LoadTextureButton { get; }

        // --- Normal map ---
        public CheckBox UseNormalMapCheckBox { get; }
        public Button LoadNormalMapButton { get; }

        // --- File ---
        public Button LoadControlPointsButton { get; }

        public ControlPanel()
        {
            Dock = DockStyle.Right;
            Width = PanelWidth;
            FlowDirection = FlowDirection.TopDown;
            WrapContents = false;
            AutoScroll = true;
            Padding = new Padding(8);
            BackColor = SystemColors.Control;

            ControlFactory.AddHeader(this, "Triangulation");
            StepsTrackBar = ControlFactory.AddTrackBar(this, "Steps",
                MeshBuilder.MinSteps, MaxSteps, SceneModel.DefaultSteps, ItemWidth, FormatInteger);

            ControlFactory.AddHeader(this, "Display");
            BezierPolygonCheckBox = ControlFactory.AddCheckBox(this, "Bezier polygon", true);
            TriangleMeshCheckBox = ControlFactory.AddCheckBox(this, "Triangle mesh", true);
            FilledTrianglesCheckBox = ControlFactory.AddCheckBox(this, "Filled triangles", false);

            ControlFactory.AddHeader(this, "Lighting");
            KdTrackBar = ControlFactory.AddTrackBar(this, "kd", 0, 100, 50, ItemWidth, FormatHundredths);
            KsTrackBar = ControlFactory.AddTrackBar(this, "ks", 0, 100, 50, ItemWidth, FormatHundredths);
            MTrackBar = ControlFactory.AddTrackBar(this, "m", 1, 100, 20, ItemWidth, FormatInteger);
            LightColorButton = ControlFactory.AddButton(this, "Light color...", ItemWidth);

            ControlFactory.AddHeader(this, "Light source");
            LightZTrackBar = ControlFactory.AddTrackBar(this, "Light Z", 0, 1000, 500, ItemWidth, FormatInteger);
            AnimateLightCheckBox = ControlFactory.AddCheckBox(this, "Animate light", true);
            ShowLightCurveCheckBox = ControlFactory.AddCheckBox(this, "Show light curve points", true);

            ControlFactory.AddHeader(this, "Object color");
            ConstantColorRadioButton = ControlFactory.AddRadioButton(this, "Constant color", true);
            ChooseColorButton = ControlFactory.AddButton(this, "Choose color...", ItemWidth);
            TextureRadioButton = ControlFactory.AddRadioButton(this, "Texture", false);
            LoadTextureButton = ControlFactory.AddButton(this, "Load texture...", ItemWidth);

            ControlFactory.AddHeader(this, "Normal map");
            UseNormalMapCheckBox = ControlFactory.AddCheckBox(this, "Use normal map", false);
            LoadNormalMapButton = ControlFactory.AddButton(this, "Load normal map...", ItemWidth);

            ControlFactory.AddHeader(this, "File");
            LoadControlPointsButton = ControlFactory.AddButton(this, "Load control points...", ItemWidth);
        }

        private static string FormatInteger(int value) => value.ToString();

        // Suwak ma wartości 0..100, a pokazujemy 0.00..1.00 (np. kd = 0.50).
        private static string FormatHundredths(int value) =>
            (value / 100f).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
    }
}
