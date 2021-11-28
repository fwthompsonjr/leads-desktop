using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.DriverFactory;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    /// <summary>
    /// Class definition for <cref="HarrisCriminalStarting">HarrisCriminalStarting</cref> class 
    /// which is used to prepare application with needed datasets and initialize data upon start
    /// </summary>
    public static class HarrisCriminalStarting
    {
        public static async Task StartAsync()
        {
            // get latest monthly records
            // get criminal records for last 30 days
            await FetchCaseStylesAsync().ConfigureAwait(false);
        }


        private static async Task FetchCaseStylesAsync()
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
            var obj = new HarrisCriminalCaseStyle();
            IWebDriver driver = GetDriver(true);
            var result = new List<HarrisCriminalStyleDto>();
            try
            {
                foreach (var dateRange in dtes)
                {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
                    var records = await Task.Run(() => { return obj.GetCases(driver, dateRange.Key, dateRange.Value); });
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
                    result.Append(records);
                }
            }
            finally
            {
                driver?.Close();
                driver?.Quit();
                obj.Dispose();
                KillProcess("chromedriver");
            }
        }


        private static IWebDriver GetDriver(bool headless = false)
        {
            var provider = new ChromeOlderProvider();
            IWebDriver driver = provider.GetWebDriver(headless);
            return driver;
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
