using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Db;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Parsing;

namespace Thompson.RecordSearch.Utility.Web
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings",
        Justification = "<Pending>")]
    [ExcludeFromCodeCoverage]
    public class HarrisCriminalCaseStyle : HarrisCriminalData
    {
        /// <summary>
        /// The user identity
        /// </summary>
        private const string uid = "frank.thompson.jr@gmail.com";

        /// <summary>
        /// The user credential
        /// </summary>
        private const string pwd = "123William890";

        /// <summary>
        /// The web address for criminal records search
        /// </summary>
        private const string url = "https://www.hcdistrictclerk.com/eDocs/Public/Search.aspx?Tab=tabCriminal";

        private static class Controls
        {
            private const string ContentPlaceHolder = "ctl00_ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder2_ContentPlaceHolder2";
            private const string Div_SearchResult = ContentPlaceHolder + "_pnlSearchResult";
            /// <summary>
            /// The login button
            /// </summary>
            public const string Btn_Login = "btnLoginImageButton";
            /// <summary>
            /// The search button
            /// </summary>
            public const string Btn_Search = ContentPlaceHolder + "_btnSearch";
            /// <summary>
            /// The search again button
            /// </summary>
            public const string Btn_SearchAgain = ContentPlaceHolder + "_btnSearchAgainTop";
            /// <summary>
            /// The print button
            /// </summary>
            public const string Btn_Print = ContentPlaceHolder + "_lblPrintResult";
            /// <summary>
            /// The textbox username
            /// </summary>
            public const string Text_UserName = "txtUserName";
            /// <summary>
            /// The textbox password
            /// </summary>
            public const string Text_Password = "txtPassword";
            /// <summary>
            /// The textbox case number
            /// </summary>
            public const string Text_CaseNo = ContentPlaceHolder + "_tabSearch_tabCriminal_txtCrimCaseNumber";
            /// <summary>
            /// The textbox case status
            /// </summary>
            public const string Text_CaseStatus = ContentPlaceHolder + "_tabSearch_tabCriminal_ddlCriminalCaseStatus";
            /// <summary>
            /// The textbox start date
            /// </summary>
            public const string Text_StartDate = ContentPlaceHolder + "_tabSearch_tabCriminal_txtCrimStartDate";
            /// <summary>
            /// The textbox end date
            /// </summary>
            public const string Text_EndDate = ContentPlaceHolder + "_tabSearch_tabCriminal_txtCrimEndDate";


            /// <summary>
            /// The textbox public image number
            /// </summary>
            public const string Text_PublicImageNbr = ContentPlaceHolder + "_tabSearch_tabCriminal_txtPubImgNbrCrim";

            /// <summary>
            /// The textbox pin number
            /// </summary>
            public const string Text_PinNbr = ContentPlaceHolder + "_txtPin";
            /// <summary>
            /// The table search
            /// </summary>
            public static string Table_Search = @"//*[@id='" + Div_SearchResult + "']/table[1]/tbody/tr[4]/td/table/tbody";
            /// <summary>
            /// The table rows
            /// </summary>
            public static string Table_Rows = Table_Search + "/tr";
            /// <summary>
            /// The table print
            /// </summary>
            public static string Table_Print = @"//table[@id='tblResults']/tbody";
            /// <summary>
            /// The table print rows
            /// </summary>
            public static string Table_PrintRows = Table_Print + "/tr[@style]";
        }

        private static class Selectors
        {
            /// <summary>
            /// Gets the username control.
            /// </summary>
            /// <value>
            /// The name of the user.
            /// </value>
            public static By UserName => By.CssSelector("#txtUserName");

            /// <summary>
            /// Gets the password control.
            /// </summary>
            /// <value>
            /// The password.
            /// </value>
            public static By Password => By.CssSelector("#txtPassword");

            /// <summary>
            /// Gets the login button.
            /// </summary>
            /// <value>
            /// The login.
            /// </value>
            public static By Login => By.Id(Controls.Btn_Login);

            /// <summary>
            /// Gets the case number.
            /// </summary>
            /// <value>
            /// The case number.
            /// </value>
            public static By CaseNumber => By.Id(Controls.Text_CaseNo);

            /// <summary>
            /// Gets the case status.
            /// </summary>
            /// <value>
            /// The case status.
            /// </value>
            public static By CaseStatus => By.Id(Controls.Text_CaseStatus);

            /// <summary>
            /// Gets the start date.
            /// </summary>
            /// <value>
            /// The start date.
            /// </value>
            public static By StartDate => By.Id(Controls.Text_StartDate);

            /// <summary>
            /// Gets the end date.
            /// </summary>
            /// <value>
            /// The end date.
            /// </value>
            public static By EndDate => By.Id(Controls.Text_EndDate);

            /// <summary>
            /// Gets the search button.
            /// </summary>
            /// <value>
            /// The search.
            /// </value>
            public static By Search => By.Id(Controls.Btn_Search);

            /// <summary>
            /// Gets the search again button.
            /// </summary>
            /// <value>
            /// The search.
            /// </value>
            public static By SearchAgain => By.Id(Controls.Btn_SearchAgain);

            /// <summary>
            /// Gets the print button.
            /// </summary>
            /// <value>
            /// The print.
            /// </value>
            public static By Print => By.Id(Controls.Btn_Print);

            /// <summary>
            /// Gets the print table.
            /// </summary>
            /// <value>
            /// The print table.
            /// </value>
            public static By PrintTable => By.XPath(Controls.Table_Print);

            /// <summary>
            /// Gets the print rows.
            /// </summary>
            /// <value>
            /// The print rows.
            /// </value>
            public static By PrintRows => By.XPath(Controls.Table_PrintRows);
            /// <summary>
            /// Gets the textbox PIN number.
            /// </summary>
            /// <value>
            /// The print rows.
            /// </value>
            public static By PinNbr => By.XPath(Controls.Text_PinNbr);

            /// <summary>
            /// Gets the textbox public image number.
            /// </summary>
            /// <value>
            /// The print rows.
            /// </value>
            public static By PublicImageNbr => By.XPath(Controls.Text_PublicImageNbr);

            /// <summary>
            /// Gets the table.
            /// </summary>
            /// <value>
            /// The table.
            /// </value>
            public static By Table => By.XPath(Controls.Table_Search);

            /// <summary>
            /// Gets the table rows.
            /// </summary>
            /// <value>
            /// The table rows.
            /// </value>
            public static By TableRows => By.XPath(Controls.Table_Rows);

            /// <summary>
            /// Gets the i frame.
            /// </summary>
            /// <value>
            /// The i frame.
            /// </value>
            public static By IFrame => By.Id("ctl00_ctl00_ctl00_TopLoginIFrame1_iFrameContent2");
        }

        /// <summary>
        /// Gets case style details from remote source for a specific range of dates.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public List<HarrisCriminalStyleDto> GetCases(IWebDriver driver, DateTime startDate, DateTime endDate)
        {
            var result = new List<HarrisCriminalStyleDto>();
            var interval = startDate.Subtract(endDate);
            var list = HarrisCaseDateDto.BuildList(
                startDate, 
                interval, 
                Convert.ToInt32(Math.Abs(interval.TotalDays)));
            driver = GetOrSetInternalDriver(driver);
            driver.Navigate().GoToUrl(url);
            driver.WaitForNavigation();

            Login(driver);

            if (!driver.IsElementPresent(Selectors.CaseNumber))
            {
                return result;
            }

            foreach (var dto in list)
            {
                result.Append(PopulateDates(dto));
            }
            return result;
        }

        /// <summary>
        /// Gets case style details from remote source for a specific range of dates.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="totalDays">The total days from the start date to search</param>
        /// <returns></returns>
        public List<HarrisCriminalStyleDto> GetCases(IWebDriver driver, DateTime startDate, int totalDays = 180)
        {
            var result = new List<HarrisCriminalStyleDto>();
            var interval = new TimeSpan(-7, 0, 0, 0);
            var list = HarrisCaseDateDto.BuildList(startDate, interval, totalDays);
            driver = GetOrSetInternalDriver(driver);
            driver.Navigate().GoToUrl(url);
            driver.WaitForNavigation();

            Login(driver);

            if (!driver.IsElementPresent(Selectors.CaseNumber))
            {
                return result;
            }

            foreach (var dto in list)
            {
                result.Append(PopulateDates(dto));
            }
            return result;
        }

        /// <summary>
        /// Gets case style details from remote source for a specific case number.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="searchDto">The search parameters containing case number and date filed.</param>
        /// <returns></returns>
        public List<HarrisCriminalStyleDto> GetData(IWebDriver driver, HarrisCaseSearchDto searchDto)
        {
            var result = new List<HarrisCriminalStyleDto>();
            var caseNumber = searchDto?.CaseNumber ?? string.Empty;
            driver = GetOrSetInternalDriver(driver);
            driver.Navigate().GoToUrl(url);
            driver.WaitForNavigation();

            Login(driver);

            if (!driver.IsElementPresent(Selectors.CaseNumber))
            {
                return result;
            }
            var txCaseNumber = driver.FindElement(Selectors.CaseNumber);
            driver.ClickAndOrSetText(txCaseNumber, caseNumber);
            if (!driver.IsElementPresent(Selectors.Search))
            {
                return result;
            }
            var btnSearch = driver.FindElement(Selectors.Search);
            driver.ClickAndOrSetText(btnSearch);
            PopulateWhenPresent(Selectors.StartDate, searchDto?.DateFiled, 0);
            PopulateWhenPresent(Selectors.EndDate, searchDto?.DateFiled, 1);
            if (!driver.IsElementPresent(Selectors.Table))
            {
                return result;
            }
            var rows = driver.FindElements(Selectors.TableRows).ToList();
            foreach (var item in rows)
            {
                var dto = ReadTable(rows, item);
                if (dto != null) { result.Add(dto); }
            }
            return result;
        }

        private IWebDriver GetOrSetInternalDriver(IWebDriver driver)
        {
            if (driver == null)
            {
                if (TheDriver == null)
                {
                    TheDriver = GetDriver();
                }
                driver = TheDriver;
            }
            else
            {
                TheDriver = driver;
            }

            return driver;
        }

        private void PopulateWhenPresent(By selector, string dateFiled, int incrementDays = 0)
        {
            if (string.IsNullOrEmpty(dateFiled))
            {
                return;
            }
            var culture = CultureInfo.InvariantCulture;
            var style = DateTimeStyles.AssumeLocal;
            if (!DateTime.TryParseExact(dateFiled, "MM/dd/yyyy", culture, style, out DateTime date))
            {
                return;
            }
            var control = TheDriver.FindElement(selector, 1);
            if (control == null)
            {
                return;
            }
            var dateFmt = date.AddDays(incrementDays).ToString("MM/dd/yyyy", culture);
            TheDriver.SetText(control, dateFmt);
        }


        private string TryGetCompleteHtml(int recordCount, DateTime stopDateTime)
        {
            var html = TheDriver.FindElement(Selectors.PrintTable).GetAttribute("outerHTML");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var parentNode = doc.DocumentNode.FirstChild;
            var nodes = parentNode.SelectNodes("tr[@style]").Cast<HtmlNode>().ToList();
            if (nodes.Count == recordCount | stopDateTime < DateTime.Now)
            {
                return html;
            }
            Thread.Sleep(250);
            return string.Empty;
        }

        [ExcludeFromCodeCoverage]
        private static int GetRecordCount(string text)
        {
            const int notfound = -1;
            if (string.IsNullOrEmpty(text)) return notfound;
            if (!text.Contains(" ")) return notfound;
            var count = text.Split(' ').ToList().Last().Replace(".", "");
            if (int.TryParse(count, out int rcount))
            {
                return rcount;
            }
            return notfound;
        }

        [ExcludeFromCodeCoverage]
        private List<HarrisCriminalStyleDto> PopulateDates(HarrisCaseDateDto dateDto)
        {
            const string format = "MM/dd/yyyy";
            const string fileFormat = "yyyy-MM-dd";
            const string status = "Active - CRIMINAL";
            string[] controlNames = new string[] { "Case Status", "Print Button", "Print Results Table" };

            var driver = TheDriver;
            var executor = (IJavaScriptExecutor)driver;
            var currentWindow = driver.CurrentWindowHandle;
            var culture = CultureInfo.InvariantCulture;
            var dates = new List<DateTime> { dateDto.StartDate, dateDto.EndDate };
            var startDate = dates.Min().ToString(format, culture);
            var endingDate = dates.Max().ToString(format, culture);
            var fsStartDate = dates.Min().ToString(fileFormat, culture);
            var fsEndingDate = dates.Max().ToString(fileFormat, culture);
            var result = new List<HarrisCriminalStyleDto>();
            var threeMinutes = TimeSpan.FromSeconds(180);
            string strFileName = $"{fsStartDate}_{fsEndingDate}_HarrisCriminalStyleDto.json";
            if (DataPersistence.FileExists(strFileName))
            {
                return DataPersistence.GetContent<List<HarrisCriminalStyleDto>>(strFileName);
            }
            driver.Manage().Timeouts().PageLoad = threeMinutes;

            // populate form
            PopulateWhenPresent(Selectors.StartDate, startDate);
            PopulateWhenPresent(Selectors.EndDate, endingDate);
            if (!IsElementPresent(driver, Selectors.CaseStatus, controlNames[0]))
            {
                throw new ElementNotInteractableException($"{controlNames[0]} Element is not found.");
            }
            var btnSearch = driver.FindElement(Selectors.Search);
            var cboCaseStatus = new SelectElement(driver.FindElement(Selectors.CaseStatus));
            cboCaseStatus.SelectByText(status);

            // submit form ... this occassionally can take longer than 30 seconds
            // so we need to click element through wed-driver and not javascript
            executor.ExecuteScript("arguments[0].scrollIntoView(true);", btnSearch);
            btnSearch.Click();
            driver.WaitForNavigation();
            const string searchRecordCount = "//*[@id=\"ctl00_ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder2_ContentPlaceHolder2_recCountCas\"]";
            var recordCount = GetRecordCount(driver.FindElement(By.XPath(searchRecordCount)).Text);
            // print results to new window
            if (!IsElementPresent(driver, Selectors.Print, controlNames[1]))
            {
                throw new ElementNotInteractableException($"{controlNames[1]} Element is not found.");
            }
            var btnPrint = driver.FindElement(Selectors.Print);
            driver.ClickAndOrSetText(btnPrint);
            string printWindowHandle = FindWindow(driver, currentWindow);
            driver.SwitchTo().Window(printWindowHandle);

            // read results from table
            if (!IsElementPresent(driver, Selectors.PrintTable, controlNames[2]))
            {
                throw new ElementNotInteractableException($"{controlNames[2]} Element is not found.");
            }

            var tableHtml =
                recordCount < 0 ?
                driver.FindElement(Selectors.PrintTable).GetAttribute("outerHTML") :
                string.Empty;

            var waitFor = TimeSpan.FromSeconds(30);
            var stopDateTime = DateTime.Now.Add(waitFor);
            while (string.IsNullOrEmpty(tableHtml))
            {
                tableHtml = TryGetCompleteHtml(recordCount, stopDateTime);
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(tableHtml);
            var parentNode = doc.DocumentNode.FirstChild;
            var nodes = parentNode.SelectNodes("tr[@style]").Cast<HtmlNode>().ToList();
            var rowdata = nodes.Select(x => x.ChildNodes.Cast<HtmlNode>().ToList())
                .ToList();
            var rowinfo = new List<IEnumerable<string>>();
            rowdata.ForEach(r =>
            {
                var txts = r.Select(c => c.InnerText);
                rowinfo.Add(txts);

            });

            foreach (var item in rowinfo)
            {
                var dto = ReadTable(rowinfo, item);
                if (dto != null) { result.Add(dto); }
            }


            driver.Close(); // close the pop-up window
            // return to main window
            driver.SwitchTo().Window(currentWindow);

            if (!IsElementPresent(driver, Selectors.SearchAgain))
            {
                throw new ElementNotInteractableException($"Expected Element 'Search Again' is not found.");
            }
            driver.ClickAndOrSetText(driver.FindElement(Selectors.SearchAgain));

            // write result to file here
            if (DataPersistence.FileExists(strFileName))
                return result;

            DataPersistence.Save(strFileName, result);
            return result;
        }

        [ExcludeFromCodeCoverage]
        private static HarrisCriminalStyleDto Parse(HarrisCriminalStyleDto dto)
        {
            if (dto == null) return null;
            dto.Style = Regex.Replace(dto.Style, @"\u00A0", " ");
            dto.Style = Regex.Replace(dto.Style, @"&nbsp;", " ");
            var parser = new CaseStyleDbParser { Data = dto.Style };
            if (!parser.CanParse()) return dto;
            var parseResult = parser.Parse();
            dto.Style = parseResult.CaseData;
            dto.Plantiff = parseResult.Plantiff;
            dto.Defendant = parseResult.Defendant;
            return dto;
        }

        [ExcludeFromCodeCoverage]
        private static string ParseText(string text, string separator)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(separator))
            {
                return text;
            }
            text = Regex.Replace(text, @"\u00A0", " ");
            if (text.Contains(separator))
            {
                return text.Split(separator.ToCharArray())[0];
            }
            return new string(text.TakeWhile(c => !Char.IsLetter(c)).ToArray());
        }

        [ExcludeFromCodeCoverage]
        private static void Login(IWebDriver driver)
        {
            try
            {
                const int timeout = 5;
                var bxSearch = IsElementPresent(driver, Selectors.CaseNumber, "Case Number");
                if (bxSearch)
                {
                    return;
                }
                string currentWindow = driver.CurrentWindowHandle;
                var frame = driver.FindElement(Selectors.IFrame);
                driver.SwitchTo().Frame(frame);
                var txUserName = driver.FindElement(Selectors.UserName);
                var txPassword = driver.FindElement(Selectors.Password);
                var btnLogin = driver.FindElement(Selectors.Login, timeout);

                driver.ClickAndOrSetText(txUserName, uid);
                driver.ClickAndOrSetText(txPassword, pwd);
                driver.ClickAndOrSetText(btnLogin);
                driver.SwitchTo().Window(currentWindow);
                driver.WaitForNavigation();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // hide these exceptions
                return;
            }
        }

        [ExcludeFromCodeCoverage]
        private static HarrisCriminalStyleDto ReadTable(List<IWebElement> rows, IWebElement item)
        {
            var index = rows.IndexOf(item);
            if (index == 0) { return default; }
            var cells = item.FindElements(By.TagName("td"));
            if (cells.Count < 6) { return default; } // in the print layout there are separator rows
            return Parse(new HarrisCriminalStyleDto
            {
                Index = index,
                CaseNumber = ParseText(cells[0].Text, Environment.NewLine),
                Style = cells[1].Text,
                FileDate = cells[2].Text,
                Court = cells[3].Text,
                Status = cells[4].Text,
                TypeOfActionOrOffense = cells[5].Text
            });
        }
        [ExcludeFromCodeCoverage]
        private static HarrisCriminalStyleDto ReadTable(List<IEnumerable<string>> rows, IEnumerable<string> item)
        {
            var index = rows.IndexOf(item);
            var data = item.ToList();
            return Parse(new HarrisCriminalStyleDto
            {
                Index = index,
                CaseNumber = ParseText(data[1], Environment.NewLine).Trim(),
                Style = data[3].Trim(),
                FileDate = data[5].Trim(),
                Court = data[7].Trim(),
                Status = data[9].Trim(),
                TypeOfActionOrOffense = data[11].Trim()
            });
        }
        [ExcludeFromCodeCoverage]
        private static string FindWindow(IWebDriver driver, string topWindowHandle)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var handles = driver.WindowHandles.ToList();
            return handles.FirstOrDefault(a => !a.Equals(topWindowHandle, oic));

        }
        [ExcludeFromCodeCoverage]
        private static bool IsElementPresent(IWebDriver driver, By by, string friendlyName = "Element")
        {
            try
            {
                var assertion = new ElementAssertion(driver);
                assertion.WaitForElementExist(by, friendlyName);
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

    }
}
