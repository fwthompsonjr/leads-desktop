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
                Interval = 5000
            };
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine("this implementation removed for process performance issue");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Qube",
            "S1144:Unused private types or members should be removed",
            Justification = "Researching if this method can be safely removed")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality",
            "IDE0051:Remove unused private members",
            Justification = "Researching if this method can be safely removed")]
        private void Execute()
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
                        Thread.Sleep(500);
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
