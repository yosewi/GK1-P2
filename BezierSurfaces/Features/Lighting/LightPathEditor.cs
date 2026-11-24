using System.Numerics;
using BezierSurfaces.Rendering;

namespace BezierSurfaces.Features.Lighting
{
    // Edycja krzywej światła: przeciąganie jej punktów kontrolnych lewym przyciskiem myszy.
    // Działa tylko wtedy, gdy punkty kontrolne są widoczne na ekranie.
    public sealed class LightPathEditor
    {
        private const float GrabRadius = 8f;

        private readonly Control _control;
        private readonly LightPath _path;

        private int _draggedIndex = -1;

        public bool IsEnabled { get; set; } = true;

        public event EventHandler? PathChanged;

        // Podpina się pod zdarzenia myszy obszaru rysowania.
        public LightPathEditor(Control control, LightPath path)
        {
            _control = control;
            _path = path;

            control.MouseDown += OnMouseDown;
            control.MouseMove += OnMouseMove;
            control.MouseUp += OnMouseUp;
        }

        public bool CanGrabAt(Point location)
        {
            return IsEnabled && FindPointAt(location) >= 0;
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsEnabled)
            {
                _draggedIndex = FindPointAt(e.Location);
            }
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (_draggedIndex < 0)
            {
                _control.Cursor = CanGrabAt(e.Location) ? Cursors.Hand : Cursors.Default;
                return;
            }

            _path.ControlPoints[_draggedIndex] = ToScene(e.Location);
            PathChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _draggedIndex = -1;
            }
        }

        private int FindPointAt(Point location)
        {
            Vector2 scenePoint = ToScene(location);
            int nearestIndex = -1;
            float nearestDistance = GrabRadius;

            for (int index = 0; index < _path.ControlPoints.Count; index++)
            {
                float distance = Vector2.Distance(_path.ControlPoints[index], scenePoint);
                if (distance <= nearestDistance)
                {
                    nearestIndex = index;
                    nearestDistance = distance;
                }
            }

            return nearestIndex;
        }

        // Zamienia piksel ekranu na punkt sceny (x, y).
        private Vector2 ToScene(Point location)
        {
            ScreenMapper mapper = new ScreenMapper(_control.ClientSize.Width, _control.ClientSize.Height);
            Vector3 point = mapper.ToWorld(location.X, location.Y, 0);
            return new Vector2(point.X, point.Y);
        }
    }
}
