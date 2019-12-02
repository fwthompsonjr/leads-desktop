using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Thompson.RecordSearch.Utility.Addressing;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class WebUtilities
    {

        /// <summary>
        /// Populates search parameters into target
        /// Executes search
        /// and reads the results to gets the cases associated.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A list of case information including person information associated to each case</returns>
        public static List<HLinkDataRow> GetCases(WebInteractive data)
        {
            var cases = new List<HLinkDataRow>();
            var target = data.Parameters.Keys.First(n => n.Name == "baseUri");
            if (target == null) return cases;
            var query = data.Parameters.Keys.First(n => n.Name == "query");
            if (query == null) return cases;
            var navTo = string.Format("{0}?{1}", target.Value, query.Value);
            using (var driver = GetWebDriver())
            {
                try
                {
                    IWebElement tbResult = null;
                    var helper = new ElementAssertion(driver);
                    // 
                    tbResult = GetCaseData(data, ref cases, navTo, helper);

                    var people = cases.FindAll(x => !string.IsNullOrEmpty(x.Uri));// .Take(4).ToList();
                    people.ForEach(d => Find(driver, data, d));
                    var found = people.Count(p => !string.IsNullOrEmpty(p.Defendant));

                }
                catch
                {
                    driver.Quit();
                    throw;
                }
                finally
                {
                    driver.Close();
                    driver.Quit();
                } 
            }
            return cases;
        }

        private static IWebElement GetCaseData(WebInteractive data, 
            ref List<HLinkDataRow> cases, 
            string navTo, ElementAssertion helper)
        {
            IWebElement tbResult;
            helper.Navigate(navTo);
            tbResult = helper.Process(data);
            var rows = tbResult.FindElements(By.TagName("tr"));
            foreach (var rw in rows)
            {
                var html = rw.GetAttribute("outerHTML");
                var link = TryFindElement(rw, By.TagName("a"));
                var address = link == null ? string.Empty : link.GetAttribute("href");
                var dataRow = new HLinkDataRow { Data = html, Uri = address };
                if (link != null) {
                    dataRow.Case = link.Text.Trim();
                }
                cases.Add(new HLinkDataRow { Data = html, Uri = address });
            }

            return tbResult;
        }


        /// <summary>
        /// Method to inspect the case result record and drill-down 
        /// to the case detail and extract the address information from the page
        /// </summary>
        /// <param name="driver">the current browser instance with automation hooks</param>
        /// <param name="webInteractive">the wrapper for the users inbound parameters and any navigation instructions needed to read the website</param>
        /// <param name="linkData">the html of the source case result record</param>
        internal static void Find(IWebDriver driver, WebInteractive webInteractive, HLinkDataRow linkData)
        {

            var finders = new List<FindDefendantBase>
            {
                new FindDefendantNavigation(),
                new FindMultipleDefendantMatch(),
                new FindDefendantByWordMatch(),
                new FindPrincipalByWordMatch(),
                new FindPetitionerByWordMatch(),
                new FindRespondentByWordMatch(),
                new FindDefendantByCondemneeMatch(),
                new FindApplicantByWordMatch(),
                new NoFoundMatch()
            };
            foreach (var finder in finders)
            {
                finder.Find(driver, linkData);
                if (finder.CanFind) break;
            }
        }

        /// <summary>
        /// Locates the address element from the case-detail drill down page.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="trCol">The tr col.</param>
        /// <returns></returns>
        private static IWebElement GetAddressRow(IWebElement parent, System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> trCol)
        {
            int colIndex = 3;
            parent = trCol[colIndex];
            if (parent.Text.Trim() == string.Empty) parent = trCol[colIndex - 1];
            return parent;
        }


        public static IWebDriver GetHiddenWebDriver()
        {
            ChromeOptions option = new ChromeOptions();
            option.AddArgument("--headless");
            var driver = new ChromeDriver(GetChromeFileName(), option);
            HideWindow("chromedriver");
            return driver;
        }

        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public static IWebDriver GetWebDriver()
        {
            var driver = GetDefaultDriver();
            if (driver != null) return driver;
            return new ChromeDriver(GetChromeFileName());
        }

        /// <summary>
        /// Gets the default driver.
        /// </summary>
        /// <returns></returns>
        private static IWebDriver GetDefaultDriver()
        {
            try
            {
                return new ChromeDriver();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the name of the chrome driver file.
        /// </summary>
        /// <returns></returns>
        private static string GetChromeFileName()
        {
            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            if (!Directory.Exists(execName)) return string.Empty;
            return execName;
        }

        /// <summary>
        /// Tries the find a child element using the By condition supplied.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <param name="by">The by condition used to locate the element</param>
        /// <returns></returns>
        private static IWebElement TryFindElement(IWebElement parent, By by)
        {
            try
            {
                return parent.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Tries the find element on a specfic web page using the By condition supplied.
        /// </summary>
        /// <param name="parent">The parent web browser instance.</param>
        /// <param name="by">The by condition used to locate the element</param>
        /// <returns></returns>
        private static IWebElement TryFindElement(IWebDriver parent, By by)
        {
            try
            {
                return parent.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        private static void HideWindow(string windowTitle)
        {

            var processlist = Process.GetProcesses().ToList()
                .FindAll(x => !string.IsNullOrEmpty(x.MainWindowTitle))
                .FindAll(x => x.ProcessName.Contains(windowTitle));

            foreach (Process process in processlist)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    var hWnd = process.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }
            }
        }
    }
}
