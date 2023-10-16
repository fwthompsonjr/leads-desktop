using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.DriverFactory
{
    public abstract class BaseChromeProvider
    {

        private static string _binaryName;
        private static string _driverFileName;
        private static string _downloadPath;

        /// <summary>
        /// Gets the download path.
        /// </summary>
        /// <value>
        /// The download path.
        /// </value>
        public static string DownloadPath => CalculateDownloadPath();

        /// <summary>
        /// Gets the chrome options.
        /// </summary>
        /// <returns></returns>
        protected static ChromeOptions GetChromeOptions()
        {

            var options = new ChromeOptions();
            var binaryName = BinaryFileName();
            if (!string.IsNullOrEmpty(binaryName))
            {
                options.BinaryLocation = binaryName;
            }
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("download.directory_upgrade", true);
            options.AddUserProfilePreference("download.default_directory", CalculateDownloadPath());
            return options;
        }

        /// <summary>
        /// Gets the name of the binary file.
        /// </summary>
        /// <returns></returns>
        protected static string BinaryFileName()
        {
            if (_binaryName != null)
            {
                return _binaryName;
            }

            _binaryName = WebUtilities.GetChromeBinary();
            return _binaryName;
        }

        /// <summary>
        /// Gets the name of the chrome driver file.
        /// </summary>
        /// <returns></returns>
        protected static string GetDriverFileName()
        {
            if (_driverFileName != null)
            {
                return _driverFileName;
            }

            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            if (!Directory.Exists(execName))
            {
                _driverFileName = string.Empty;
                return string.Empty;
            }
            _driverFileName = execName;
            return execName;
        }

        /// <summary>
        /// Gets the name of the chrome download directory.
        /// </summary>
        /// <returns></returns>
        protected static string CalculateDownloadPath()
        {
            if (_downloadPath != null)
            {
                return _downloadPath;
            }

            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            if (!Directory.Exists(execName))
            {
                return string.Empty;
            }

            execName = Path.Combine(execName, "_downloads");
            if (Directory.Exists(execName))
            {
                _downloadPath = execName;
                return _downloadPath;
            }
            Directory.CreateDirectory(execName);
            _downloadPath = execName;
            return execName;
        }

    }
}
