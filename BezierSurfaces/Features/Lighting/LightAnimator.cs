using System.Diagnostics;

namespace BezierSurfaces.Features.Lighting
{
    // Przesuwa światło po krzywej o tyle, ile czasu naprawdę upłynęło od poprzedniej klatki.
    // Dzięki temu prędkość ruchu nie zależy od tego, jak często rysują się klatki.
    public sealed class LightAnimator
    {
        // Prędkość: jedno okrążenie na 10 sekund.
        private const float LapsPerSecond = 0.1f;

        // Najdłuższy czas brany pod uwagę między klatkami. Po dłuższej przerwie
        // (np. gdy nic nie było rysowane) światło nie przeskakuje daleko do przodu.
        private const double MaxFrameSeconds = 0.1;

        private readonly Stopwatch _clock = Stopwatch.StartNew();
        private double _lastFrameSeconds;

        // Czy światło ma się poruszać.
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
