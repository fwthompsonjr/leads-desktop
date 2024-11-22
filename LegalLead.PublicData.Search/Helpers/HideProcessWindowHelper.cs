using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace LegalLead.PublicData.Search.Helpers
{
    internal class HideProcessWindowHelper : IDisposable
    {
        public HideProcessWindowHelper()
        {
            _timer = new System.Timers.Timer
            {
                Interval = 500
            };
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_timerLock)
            {
                var processes = Process.GetProcessesByName("geckodriver")?.ToList();
                if (processes == null || processes.Count == 0) return;
                foreach (var p in processes)
                {
                    while (p.MainWindowHandle == IntPtr.Zero)
                    {
                        p.Refresh();
                        Thread.Sleep(100);
                    }
                    ShowWindow(p.MainWindowHandle, SW_HIDE);
                }
            }
        }

        private readonly System.Timers.Timer _timer;
        private readonly object _timerLock = new();
        private bool disposedValue;
        private const int SW_HIDE = 0;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    KillTimer();
                }
                disposedValue = true;
            }
        }

        private void KillTimer()
        {
            try
            {
                _timer.Stop();
                _timer.Close();
                _timer.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
