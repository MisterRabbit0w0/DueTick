using System;
using System.Timers;

namespace DueTick.Services
{
    public class TimerService : IDisposable
    {
        public event EventHandler Tick;

        private readonly Timer _timer;

        public TimerService()
        {
            _timer = new Timer(60000); // Check every minute
            _timer.Elapsed += (s, e) => Tick?.Invoke(this, EventArgs.Empty);
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();

        public void Dispose() => _timer.Dispose();
    }
}
