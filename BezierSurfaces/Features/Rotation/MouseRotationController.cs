namespace BezierSurfaces.Features.Rotation
{
    public sealed class MouseRotationController
    {
        private const float RadiansPerPixel = 0.01f;

        private static readonly float RadiansPerWheelNotch = SceneOrientation.DegreesToRadians(5);

        private const float WheelDeltaPerNotch = 120f;

        private readonly SceneOrientation _orientation;
        private bool _isDragging;
        private Point _lastMousePosition;

        public event EventHandler? OrientationChanged;

        public MouseRotationController(Control control, SceneOrientation orientation)
        {
            _orientation = orientation;

            control.MouseDown += OnMouseDown;
            control.MouseMove += OnMouseMove;
            control.MouseUp += OnMouseUp;
            control.MouseWheel += OnMouseWheel;

            control.MouseEnter += (sender, e) =>
            {
                Form? form = control.FindForm();
                if (form != null)
                {
                    form.ActiveControl = null;
                }
            };
        }

        private void OnMouseWheel(object? sender, MouseEventArgs e)
        {
            float notches = e.Delta / WheelDeltaPerNotch;
            _orientation.RotateAroundScreenZ(notches * RadiansPerWheelNotch);

            OrientationChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            _isDragging = true;
            _lastMousePosition = e.Location;
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDragging)
            {
                return;
            }

            int dx = e.X - _lastMousePosition.X;
            int dy = e.Y - _lastMousePosition.Y;
            _lastMousePosition = e.Location;

            if (dx == 0 && dy == 0)
            {
                return;
            }

            _orientation.RotateAroundScreenY(dx * RadiansPerPixel);
            _orientation.RotateAroundScreenX(dy * RadiansPerPixel);

            OrientationChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
            }
        }
    }
}
