using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings",
        Justification = "<Pending>")]
    public class HarrisCriminalCaseStyle : HarrisCriminalData
    {
        private static string uid = "frank.thompson.jr@gmail.com";
        private static string pwd = "123William890";
        private static string url = "https://www.hcdistrictclerk.com/eDocs/Public/Search.aspx?Tab=tabCriminal";

        private static class Controls
        {
            private const string ContentPlaceHolder = "ctl00_ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder2_ContentPlaceHolder2";
            private const string Div_SearchResult = ContentPlaceHolder + "_pnlSearchResult";

            public const string Btn_Login = "btnLoginImageButton";
            public const string Btn_Search = ContentPlaceHolder + "_btnSearch";
            public const string Text_UserName = "txtUserName";
            public const string Text_Password = "txtPassword";
            public const string Text_CaseNo = ContentPlaceHolder + "_tabSearch_tabCriminal_txtCrimCaseNumber";
            public const string Text_StartDate = ContentPlaceHolder + "_tabSearch_tabCriminal_txtCrimStartDate";
            public const string Text_EndDate = ContentPlaceHolder + "_tabSearch_tabCriminal_txtCrimEndDate";
            public static string Table_Search = @"//*[@id='" + Div_SearchResult + "']/table[1]/tbody/tr[4]/td/table/tbody";
            public static string Table_Rows = Table_Search + "/tr";
        }

        private static class Selectors
        {
            public static By UserName => By.CssSelector("#txtUserName");
            public static By Password => By.CssSelector("#txtPassword");
            public static By Login => By.Id(Controls.Btn_Login);
            public static By CaseNumber => By.Id(Controls.Text_CaseNo);
            public static By StartDate => By.Id(Controls.Text_StartDate);
            public static By EndDate => By.Id(Controls.Text_EndDate);
            public static By Search => By.Id(Controls.Btn_Search);
            public static By Table => By.XPath(Controls.Table_Search);
            public static By TableRows => By.XPath(Controls.Table_Rows);

            public static By IFrame => By.Id("ctl00_ctl00_ctl00_TopLoginIFrame1_iFrameContent2");
        }

        public List<HarrisCriminalStyleDto> GetData(IWebDriver driver, IEnumerable<HarrisCaseSearchDto> caseNumbers)
        {
            var result = new List<HarrisCriminalStyleDto>();
            if (caseNumbers == null) return result;
            var list = caseNumbers
                .GroupBy(x => x.UniqueIndex)
                .Select(x => x.FirstOrDefault())
                .ToList();
            foreach (var caseNumber in list)
            {
                var index = list.IndexOf(caseNumber);
                result.Append(GetData(driver, caseNumber));
                if (index == 1)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
            return result;
        }

        public List<HarrisCriminalStyleDto> GetData(IWebDriver driver, HarrisCaseSearchDto searchDto)
        {
            var result = new List<HarrisCriminalStyleDto>();
            var caseNumber = searchDto?.CaseNumber ?? string.Empty;
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
                var index = rows.IndexOf(item);
                if (index == 0) { continue; }
                var cells = item.FindElements(By.TagName("td"));
                var dto = new HarrisCriminalStyleDto
                {
                    Index = index,
                    CaseNumber = ParseText(cells[0].Text, Environment.NewLine),
                    Style = cells[1].Text,
                    FileDate = cells[2].Text,
                    Court = cells[3].Text,
                    Status = cells[4].Text,
                    TypeOfActionOrOffense = cells[5].Text
                };
                result.Add(dto);
            }
            return result;
        }

        private void PopulateWhenPresent(By selector, string dateFiled, int incrementDays)
        {
            if (string.IsNullOrEmpty(dateFiled))
            {
                return;
            }
            var culture = CultureInfo.InvariantCulture;
            var style = DateTimeStyles.AssumeLocal;
            if (!DateTime.TryParseExact(dateFiled, "yyyyMMdd", culture, style, out DateTime date))
            {
                return;
            }
            var control = TheDriver.FindElement(selector, 1);
            if (control == null)
            {
                return;
            }
            var dateFmt = date.AddDays(incrementDays).ToString("M/d/yyyy");
            TheDriver.ClickAndOrSetText(control, dateFmt);
        }

        private static string ParseText(string text, string separator)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(separator))
            {
                return text;
            }
            return text.Split(separator.ToCharArray())[0];
        }

        private static void Login(IWebDriver driver)
        {
            try
            {
                const int timeout = 5;
                var bxSearch = IsElementPresent(driver, Selectors.CaseNumber);
                if (bxSearch)
                {
                    return;
                }
                var frame = driver.FindElement(Selectors.IFrame);
                driver.SwitchTo().Frame(frame);
                var txUserName = driver.FindElement(Selectors.UserName);
                var txPassword = driver.FindElement(Selectors.Password);
                var btnLogin = driver.FindElement(Selectors.Login, timeout);

                driver.ClickAndOrSetText(txUserName, uid);
                driver.ClickAndOrSetText(txPassword, pwd);
                driver.ClickAndOrSetText(btnLogin);

            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // hide these exceptions
                return;
            }
        }


        private static bool IsElementPresent(IWebDriver driver, By by)
        {
            try
            {
                var assertion = new ElementAssertion(driver);
                assertion.WaitForElementExist(by, "Element");
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
