using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
        public IHccWritingService HccService { get; set; } = null;
        protected string DownloadShortName = string.Empty;
        protected string DownloadFileName = string.Empty;
        public override object Execute()
        {
            if (!IsDownloadRequested) { return true; }
            var model = HccConfigurationModel.GetModel();
            var js = FindRecordJs(model.Monthly);
            var executor = GetJavaScriptExecutor();
            DownloadShortName = $"{model.Monthly}.txt";
            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            return RequestDownload(js, executor);
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

        [ExcludeFromCodeCoverage(Justification = "Interacts with file system.")]
        protected object RequestDownload(string js, IJavaScriptExecutor executor)
        {
            try
            {
                var rsp = executor.ExecuteScript(js);
                if (rsp is bool success && !success) return false;
                DownloadFileName = WaitForDownload();
                if (HccService == null) return true;
                var csv = File.ReadAllText(DownloadFileName);
                HccService.Write(csv);
                return true;
            }
            finally
            {
                if (!string.IsNullOrEmpty(DownloadFileName) && File.Exists(DownloadFileName))
                    File.Delete(DownloadFileName);
            }
        }
        [ExcludeFromCodeCoverage(Justification = "Interacts with file system.")]
        protected string WaitForDownload()
        {
            const int initialWait = 60;
            const int secondWait = 90;
            if (IsTestMode) return string.Empty;
            if (string.IsNullOrEmpty(DownloadShortName)) return string.Empty;
            var downloadsPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\";
            if (!Directory.Exists(downloadsPath)) return string.Empty;
            var minimumDt = DateTime.UtcNow.AddSeconds(-10);
            var fullPath = FindFile(downloadsPath, DownloadShortName, minimumDt);
            for (var i = 0; i < initialWait; i++)
            {
                if (fullPath != null) { break; }
                Thread.Sleep(1000);
                fullPath = FindFile(downloadsPath, DownloadShortName, minimumDt);
            }
            if (!File.Exists(fullPath)) return string.Empty;
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
            return fullPath;
        }

        [ExcludeFromCodeCoverage(Justification = "Interacts with file system.")]
        private static string FindFile(string parentDir, string fileName, DateTime minDate)
        {
            if (!Directory.Exists(parentDir)) return null;
            var pattern = $"*{fileName}*";
            var info = new DirectoryInfo(parentDir);
            var files = info.GetFiles(pattern).ToList();
            if (files.Count == 0) return null;
            var found = files.Find(f => f.CreationTimeUtc > minDate);
            return found?.FullName ?? null;
        }
    }
}