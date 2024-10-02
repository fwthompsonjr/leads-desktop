using Harris.Criminal.Db.Entities;
using System;
using System.Diagnostics;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Db
{
    [DataAction(Name = "header", ProcessId = 1000, IsShared = true)]
    public class WebCleanUpAction : BaseAction
    {

        public WebCleanUpAction(HccProcess process) : base(process)
        {
        }

        public override TimeSpan EstimatedDuration => TimeSpan.FromSeconds(2);

        public override void Execute(IProgress<HccProcess> progress)
        {
            ReportProgress = progress;
            Start();
            CleanUp();
            End();
        }
        private void CleanUp()
        {
            try
            {
                KillChrome();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                KillProcess("chromedriver");
            }
        }
        private static void KillChrome()
        {
            var processes = Process.GetProcessesByName("chrome")
                .Where(_ => !_.MainWindowHandle.Equals(IntPtr.Zero));
            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        private static void KillProcess(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
    }
}
