using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Thompson.RecordSearch.Utility.DriverFactory;
using Thompson.RecordSearch.Utility.Web;

namespace LegalLead.PublicData.Search.Command
{
    public static class HarrisCriminalUpdate
    {
        public static bool IsDataReady { get; private set; }
        public static void Update()
        {
            IsDataReady = false;
            var monthly = new Thread(new ThreadStart(() =>
            {
                // get latest download file
                const string msg = "{0:G} - {1} background load harris criminal monthly data.";
                var culture = CultureInfo.CurrentUICulture;
                Console.WriteLine(string.Format(culture, msg, DateTime.Now, "Begin"));
                GetLatestMonthlyRecords();
                Console.WriteLine(string.Format(culture, msg, DateTime.Now, "End"));

                IsDataReady = true;
            }));
            monthly.Start();
        }

        private static void GetLatestMonthlyRecords()
        {
            using (var obj = new HarrisCriminalData())
            {
                IWebDriver driver = GetDriver(true);
                try
                {
                    _ = obj.GetData(driver);
                }
                finally
                {
                    KillDriver(driver);
                    KillProcess("chromedriver");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Intention is to suppress an exception during close/quit of iwebdriver")]
        private static void KillDriver(IWebDriver driver)
        {
            try
            {
                driver?.Close();
                driver?.Quit();
            }
            catch
            {
                // taking no action
            }
        }
        private static IWebDriver GetDriver(bool headless = true)
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
