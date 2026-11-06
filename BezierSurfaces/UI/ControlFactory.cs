namespace BezierSurfaces.UI
{
    public static class ControlFactory
    {
        private static readonly Font HeaderFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

        public static Label AddHeader(Control parent, string text)
        {
            Label label = new Label
            {
                Text = text,
                AutoSize = true,
                Font = HeaderFont,
                Margin = new Padding(0, 10, 0, 2),
            };
            parent.Controls.Add(label);
            return label;
        }

        public static TrackBar AddTrackBar(Control parent, string caption, int min, int max, int value,
            int width, Func<int, string> formatValue)
        {
            Label label = new Label
            {
                AutoSize = true,
                Text = $"{caption}: {formatValue(value)}",
                Margin = new Padding(0, 2, 0, 0),
            };

            TrackBar trackBar = new TrackBar
            {
                Minimum = min,
                Maximum = max,
                Value = value,
                TickStyle = TickStyle.None,
                AutoSize = false,
                Height = 28,
                Width = width,
            };

            trackBar.ValueChanged += (sender, e) => label.Text = $"{caption}: {formatValue(trackBar.Value)}";

            parent.Controls.Add(label);
            parent.Controls.Add(trackBar);
            return trackBar;
        }

        public static CheckBox AddCheckBox(Control parent, string text, bool isChecked)
        {
            CheckBox checkBox = new CheckBox
            {
                Text = text,
                Checked = isChecked,
                AutoSize = true,
            };
            parent.Controls.Add(checkBox);
            return checkBox;
        }

        public static RadioButton AddRadioButton(Control parent, string text, bool isChecked)
        {
            RadioButton radioButton = new RadioButton
            {
                Text = text,
                Checked = isChecked,
                AutoSize = true,
            };
            parent.Controls.Add(radioButton);
            return radioButton;
        }

        public static Button AddButton(Control parent, string text, int width)
        {
            Button button = new Button
            {
                Text = text,
                Width = width,
                Height = 26,
            };
            parent.Controls.Add(button);
            return button;
        }
    }
}
