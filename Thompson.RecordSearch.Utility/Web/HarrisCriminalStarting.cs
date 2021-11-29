using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.DriverFactory;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Tools;

namespace Thompson.RecordSearch.Utility.Web
{
    public enum CriminalStartType
    {
        All,
        CaseTypes,
        Download
    }

    /// <summary>
    /// Class definition for <cref="HarrisCriminalStarting">HarrisCriminalStarting</cref> class 
    /// which is used to prepare application with needed datasets and initialize data upon start
    /// </summary>
    public static class HarrisCriminalStarting
    {

        private static readonly IEnumerable<CriminalStartType> DefaultStart = GetDefaultStartType();

        private static IEnumerable<CriminalStartType> GetDefaultStartType()
        {
            return new List<CriminalStartType> { CriminalStartType.All }.AsEnumerable();
        }


        public static async Task StartAsync(IEnumerable<CriminalStartType> startTypes = null)
        {
            if (startTypes == null)
            {
                startTypes = DefaultStart;
            }
            IWebDriver driver = GetDriver(true);
            try
            {

                // get criminal records for last 30 days
                if (startTypes.Contains(CriminalStartType.All) || startTypes.Contains(CriminalStartType.CaseTypes))
                {
                    await FetchCaseStylesAsync(driver).ConfigureAwait(false);
                }

                // get latest monthly records
                if (startTypes.Contains(CriminalStartType.All) || startTypes.Contains(CriminalStartType.Download))
                {
                    await FetchLastDownloadAsync(driver).ConfigureAwait(false);
                }
            }
            finally
            {
                driver?.Close();
                driver?.Quit();
                KillProcess("chromedriver");
            }
        }

        private static async Task FetchLastDownloadAsync(IWebDriver driver)
        {
            using (var obj = new HarrisCriminalData())
            {
                string result = null;
                await Task.Run(() => { result = obj.GetData(driver); }).ConfigureAwait(false);
                Debug.Assert(result != null);
                Debug.Assert(File.Exists(result));
            }

        }
        private static async Task FetchCaseStylesAsync(IWebDriver driver)
        {
            const int interval = -5;
            const int cycleId = 10;
            DateTime MxDate = DateTime.Now.AddDays(-1).Date;
            DateTime MnDate = MxDate.AddDays(interval);

            var dtes = new List<KeyValuePair<DateTime, DateTime>>
            {
                new KeyValuePair<DateTime, DateTime>(MnDate, MxDate)
            };
            while (dtes.Count < cycleId)
            {
                var item = dtes.Last();
                dtes.Add(new KeyValuePair<DateTime, DateTime>(item.Key.AddDays(interval), item.Key));
            }
            using (var obj = new HarrisCriminalCaseStyle())
            {
                var result = new List<HarrisCriminalStyleDto>();
                foreach (var dateRange in dtes)
                {
                    var records = await Task.Run(() =>
                    {
                        return obj.GetCases(driver, dateRange.Key, dateRange.Value);
                    }).ConfigureAwait(false);
                    result.Append(records);
                }
            }
        }

        private static IWebDriver GetDriver(bool headless = false)
        {
            const string title = "chrome";
            var culture = CultureInfo.CurrentCulture;
            var provider = new ChromeOlderProvider();
            var originalList = ListProcess(title).Split(',')
                .Select(x => Convert.ToInt32(x.Trim(), culture));

            IWebDriver driver = provider.GetWebDriver();
            if (headless)
            {
                var processIndexes = ListProcess(title);
                Debug.WriteLine(title);
                Debug.WriteLine(processIndexes);
                var list = processIndexes.Split(',')
                .Select(x => Convert.ToInt32(x.Trim(), culture))
                .Where(y => !originalList.Contains(y));
                Debug.WriteLine(string.Join(" - ", list));
                if (list.Any())
                {
                    EnumerateWindows.HideWindow(list.First());
                }
            }
            return driver;
        }

        private static void KillProcess(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }

        private static string ListProcess(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            var handles = processes
                .Select(p => p.MainWindowHandle.ToInt32())
                .Where(x => x > 0);
            return string.Join(", ", handles);
        }
    }
}
