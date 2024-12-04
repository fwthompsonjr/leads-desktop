using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class HccDownloadMonthly : BaseWilliamsonSearchAction
    {
        public override int OrderId => 15;
        public bool IsTestMode { get; set; }
        public bool IsDownloadRequested { get; set; }

        protected string DownloadFileName = string.Empty;
        public override object Execute()
        {
            if (!IsDownloadRequested) { return true; } 
            var model = HccConfigurationModel.GetModel();
            var js = FindRecordJs(model.Monthly);
            var executor = GetJavaScriptExecutor();
            DownloadFileName = $"{model.Monthly}.txt";
            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            var rsp = executor.ExecuteScript(js);
            if (rsp is bool success && !success) return false;
            WaitForDownload();
            return true;
        }

        protected void WaitForDownload()
        {
            const int initialWait = 60;
            const int secondWait = 90;
            if (IsTestMode) return;
            if (string.IsNullOrEmpty(DownloadFileName)) return;
            var downloadsPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\";
            if (!Directory.Exists(downloadsPath)) return;
            var fullPath = Path.Combine(downloadsPath, DownloadFileName);
            for (var i = 0; i < initialWait; i++)
            {
                if (File.Exists(fullPath)) { break; }
                Thread.Sleep(1000);
            }
            if (!File.Exists(fullPath)) return;
            var sw = new Stopwatch();
            sw.Start();
            var length = new FileInfo(fullPath).Length;
            for (var i = 0; i < secondWait; i++)
            {
                Thread.Sleep(1500);
                Console.WriteLine($"Wait {sw.Elapsed}. Downloading data in progress.");
                var newLength = new FileInfo(downloadsPath).Length;
                if (newLength == length && length != 0) { break; }
                length = newLength;
            }
            sw.Stop();
        }

        protected static string FindRecordJs(string keyword)
        {
            var data = new string[]{
                "var find = '~0';".Replace("~0", keyword),
                "var tds = Array.prototype.slice.call( document.getElementsByTagName('td'), 0)",
                ".filter(x => x.innerText != null && x.innerText.indexOf(find) >= 0);",
                "if (tds.length <= 0 ) return false;",
                "var row = tds[0].closest('tr');",
                "if (!row.children || row.children.length < 2) return false;",
                "var links = row.children[2].getElementsByTagName('a');",
                "if (links == null || links.length < 1) return false;",
                "links[0].click();"
            };
            return string.Join(Environment.NewLine, data);
        }

    }
}