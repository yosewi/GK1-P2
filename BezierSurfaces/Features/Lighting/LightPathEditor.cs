using System.Numerics;
using BezierSurfaces.Rendering;

namespace BezierSurfaces.Features.Lighting
{
    // Edycja krzywej światła: przeciąganie jej punktów kontrolnych lewym przyciskiem myszy.
    // Działa tylko wtedy, gdy punkty kontrolne są widoczne na ekranie.
    public sealed class LightPathEditor
    {
        // Jak blisko punktu trzeba kliknąć, żeby go chwycić.
        private const float GrabRadius = 8f;

        private readonly Control _control;
        private readonly LightPath _path;

        // Numer przeciąganego punktu albo -1, gdy żaden nie jest przeciągany.
        private int _draggedIndex = -1;

        // Czy edycja jest włączona (punkty kontrolne są widoczne).
        public bool IsEnabled { get; set; } = true;

        // Zgłasza, że krzywa się zmieniła i trzeba przerysować scenę.
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

        // Czy w tym miejscu ekranu jest punkt, który można chwycić.
        // Obrót myszą pyta o to, żeby nie obracać sceny, gdy przeciągamy punkt.
        public bool CanGrabAt(Point location)
        {
            return IsEnabled && FindPointAt(location) >= 0;
        }

        // Wciśnięcie lewego przycisku na punkcie zaczyna jego przeciąganie.
        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && IsEnabled)
            {
                _draggedIndex = FindPointAt(e.Location);
            }
        }

        // Podczas przeciągania punkt podąża za myszą. Bez przeciągania zmieniamy tylko kursor,
        // żeby było widać, że punkt można chwycić.
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

        // Puszczenie lewego przycisku kończy przeciąganie.
        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _draggedIndex = -1;
            }
        }

        // Numer najbliższego punktu kontrolnego w promieniu chwytu albo -1, gdy żadnego tam nie ma.
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

        // Zamienia piksel ekranu na punkt sceny (x, y), tak samo jak przy rysowaniu.
        private Vector2 ToScene(Point location)
        {
            ScreenMapper mapper = new ScreenMapper(_control.ClientSize.Width, _control.ClientSize.Height);
            Vector3 point = mapper.ToWorld(location.X, location.Y, 0);
            return new Vector2(point.X, point.Y);
        }
    }
}
