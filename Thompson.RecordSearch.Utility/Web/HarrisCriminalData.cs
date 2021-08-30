using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.DriverFactory;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    public class HarrisCriminalData : IDisposable
    {
        private const string div = "ctl00_ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder2_ContentPlaceHolder2_blah";
        private static readonly Dictionary<string, string> Keys = new Dictionary<string, string>
        {
            { "address", "https://www.hcdistrictclerk.com/Common/e-services/PublicDatasets.aspx" },
            { "tr.monthly", "//tr[contains(string(), \"CrimFilingsWithFutureSettings\")]" },
            { "download", "//tr[@id='" + div + "']/table/tbody/tr[58]/td[3]/a/u/b" }

        };

        private static string _downloadFolder;

        private static string DownloadTo => _downloadFolder ?? (_downloadFolder = BaseChromeProvider.DownloadPath);


        /// <summary>
        /// Gets the download folder.
        /// </summary>
        /// <value>
        /// The download folder.
        /// </value>
        public static string DownloadFolder => DownloadTo;

        /// <summary>
        /// Gets data for Criminal Filings With Future Settings Download.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">driver</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static",
            Justification = "Plan to use DI to expose this behavior.")]
        public virtual string GetData(IWebDriver driver)
        {
            var current = DateTime.Now;
            var computedName = GetDownloadName();
            if (File.Exists(computedName))
            {
                return computedName;
            }
            if (driver == null)
            {
                driver = GetDriver();
            }
            try
            {
                var jse = (IJavaScriptExecutor)driver;
                var address = Keys["address"];
                var search = Keys["tr.monthly"];
                var link = Keys["download"];

                driver.Navigate().GoToUrl(address);
                if (!driver.IsElementPresent(By.XPath(search)))
                {
                    // get the latest file, if any
                    var latestFile = GetFiles().FirstOrDefault();
                    return latestFile?.FullName;
                }
                var trElement = driver.FindElement(By.XPath(search));
                var tdLast = trElement.FindElements(By.TagName("td")).ToList().Last();
                var anchor = tdLast.FindElement(By.TagName("a"));
                var expectedFileName = GetDownloadName(anchor);
                if (File.Exists(expectedFileName))
                {
                    return expectedFileName;
                }
                jse.ExecuteScript("arguments[0].click();", anchor);
                var trackingStart = DateTime.Now;
                var timeout = 5 * 60; // allow five minutes
                while (FileCreateComplete(trackingStart, timeout) == false) { Thread.Sleep(2000); }
                var list = GetFiles();
                if (!list.Any()) return null;
                list.Sort((a, b) => b.CreationTime.CompareTo(a.CreationTime));
                return list[0].FullName;
            }
            catch (Exception)
            {
                // Exceptiions are not reported from this method
                driver.Close();
                driver.Quit();
                return null;
            }
        }
        private static string GetDownloadName()
        {
            var currentDate = DateTime.Now;
            if (currentDate.Hour < 8)
            {
                // datasets are not expected until 5AM
                // adding code to only pull new data after 8AM
                currentDate = currentDate.AddDays(-1);
            }
            var culture = CultureInfo.InvariantCulture;
            var computed = string.Concat(currentDate.ToString("yyyy-MM-dd", culture), " CrimFilingsWithFutureSettings_withHeadings.txt");
            return Path.Combine(DownloadTo, computed);
        }

        private static string GetDownloadName(IWebElement anchor)
        {
            var onclick = anchor.GetAttribute("onclick");
            var fileName = Path.GetFileName(onclick
                .Replace("DownloadDoc('", "")
                .Replace("');", ""));
            return Path.Combine(DownloadTo, fileName);
        }

        private static bool FileCreateComplete(DateTime startTime, int timeoutInSeconds)
        {
            var trackingEnd = startTime.AddSeconds(timeoutInSeconds);
            var isTrackingTimeout = false;
            if (trackingEnd < DateTime.Now) { isTrackingTimeout = true; };

            var files = new DirectoryInfo(DownloadTo).GetFiles("*.txt");
            if (!files.Any()) return false | isTrackingTimeout; // keep looking nothing found
            var list = files.ToList()
                .FindAll(a => Path.GetExtension(a.Name).Equals(".txt", StringComparison.OrdinalIgnoreCase));
            return GetFiles().Any(a => a.CreationTime > startTime) | isTrackingTimeout;
        }

        private static List<FileInfo> GetFiles()
        {
            var files = new DirectoryInfo(DownloadTo).GetFiles("*.txt");
            if (!files.Any()) return new List<FileInfo>(); // keep looking nothing found
            var list = files.ToList();
            list.Sort((a, b) => b.CreationTime.CompareTo(a.CreationTime));
            return list
                .FindAll(a => a.Name.Contains("CrimFilingsWithFutureSettings"))
                .FindAll(a => Path.GetExtension(a.Name).Equals(".txt", StringComparison.OrdinalIgnoreCase));
        }

        protected static IWebDriver GetDriver()
        {
            var wdriver = (new WebDriverDto().Get()).WebDrivers;
            var driver = wdriver.Drivers.Where(d => d.Id == wdriver.SelectedIndex).FirstOrDefault();
            var container = WebDriverContainer.GetContainer;
            var provider = container.GetInstance<IWebDriverProvider>(driver.Name);
            var showBrowser =
                Convert.ToBoolean(
                ConfigurationManager.AppSettings["harris.criminal.show.browser"] ?? "true",
                CultureInfo.InvariantCulture);
            return provider.GetWebDriver(showBrowser);
        }

        protected IWebDriver TheDriver { get; set; }

        private bool _disposed = false;

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).            
                TheDriver?.Close();
                TheDriver?.Quit();
                TheDriver.Dispose();
                KillProcess("chromedriver");
            }

            _disposed = true;
        }


        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected static void KillProcess(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
    }
}
