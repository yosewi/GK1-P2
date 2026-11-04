namespace WinFormsApp1.UI
{
    /// <summary>
    /// Pomocnicze funkcje do tworzenia kontrolek panelu w jednolitym stylu.
    /// Każda funkcja tworzy kontrolkę, dodaje ją do panelu i zwraca ją.
    /// </summary>
    public static class ControlFactory
    {
        public static Label AddHeader(Control parent, string text)
        {
            Label label = new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold),
                Margin = new Padding(0, 10, 0, 2),
            };
            parent.Controls.Add(label);
            return label;
        }

        /// <summary>
        /// Suwak z podpisem, który pokazuje aktualną wartość, np. "kd: 0.50".
        /// formatValue zamienia wartość suwaka (int) na tekst podpisu.
        /// </summary>
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
