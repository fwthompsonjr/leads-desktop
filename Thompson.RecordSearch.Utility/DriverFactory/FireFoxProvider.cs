using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Thompson.RecordSearch.Utility.DriverFactory
{
    public class FireFoxProvider : IWebDriverProvider
    {
        public string Name => "Firefox";
        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public IWebDriver GetWebDriver(bool headless = true)
        {
            const string processName = "geckodriver";
            try
            {
                // make sure all instances of geckodriver are gone
                KillProcess(processName);
                var driver = GetDefaultDriver(headless);
                if (driver != null)
                {
                    return driver;
                }
                return new FirefoxDriver(GetDriverFileName());
            }
            finally
            {
                HideProcesses(processName);
            }
        }

        private static string _driverFileName;

        /// <summary>
        /// Gets the default driver.
        /// </summary>
        /// <returns></returns>
        private static IWebDriver GetDefaultDriver(bool headless = false)
        {
            try
            {
                var profile = new FirefoxOptions()
                {
                    AcceptInsecureCertificates = true
                };
                if (headless) profile.AddArgument("--headless");
                profile.SetPreference("browser.safebrowsing.enabled", true);
                profile.SetPreference("browser.safebrowsing.malware.enabled", true);
                profile.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
                return new FirefoxDriver(profile);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the name of the chrome driver file.
        /// </summary>
        /// <returns></returns>
        private static string GetDriverFileName()
        {
            if (_driverFileName != null)
            {
                return _driverFileName;
            }

            var execName = new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            if (!Directory.Exists(execName))
            {
                _driverFileName = string.Empty;
                return string.Empty;
            }
            _driverFileName = execName;
            return execName;
        }



        private static void KillProcess(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }

        private static void HideProcesses(string processName)
        {
            var processes = Process.GetProcessesByName(processName)?.ToList();
            if (processes == null || processes.Count == 0) return;
            processes.ForEach(prc => {
                try { ShowWindow(prc.MainWindowHandle, SW_HIDE); } 
                catch { 
                    // intentionally blank
                }
            });
        }
        private const int SW_HIDE = 0;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
