using System.Diagnostics;

namespace BezierSurfaces.Features.Lighting
{
    // Przesuwa światło po krzywej o tyle, ile czasu upłynęło od poprzedniej klatki.
    public sealed class LightAnimator
    {
        private const float LapsPerSecond = 0.1f;

        private const double MaxFrameSeconds = 0.1;

        private readonly Stopwatch _clock = Stopwatch.StartNew();
        private double _lastFrameSeconds;

        public bool IsRunning { get; set; } = true;

        // Wołane przy każdej klatce: mierzy czas od poprzedniej klatki i przesuwa światło.
        public void Update(LightSource light)
        {
            double now = _clock.Elapsed.TotalSeconds;
            double elapsed = Math.Min(now - _lastFrameSeconds, MaxFrameSeconds);
            _lastFrameSeconds = now;

            if (IsRunning)
            {
                light.Advance((float)(elapsed * LapsPerSecond));
            }
        }
    }
}
