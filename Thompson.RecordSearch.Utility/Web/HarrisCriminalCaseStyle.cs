using OpenQA.Selenium;
using System;
using System.Collections.Generic;
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
            public static string Table_Search = @"//*[@id='" + Div_SearchResult + "']/table[1]/tbody/tr[4]/td/table/tbody";
            public static string Table_Rows = Table_Search + "/tr";
        }

        private static class Selectors
        {
            public static By UserName => By.CssSelector("#txtUserName");
            public static By Password => By.CssSelector("#txtPassword");
            public static By Login => By.Id(Controls.Btn_Login);
            public static By CaseNumber => By.Id(Controls.Text_CaseNo);
            public static By Search => By.Id(Controls.Btn_Search);
            public static By Table => By.XPath(Controls.Table_Search);
            public static By TableRows => By.XPath(Controls.Table_Rows);

            public static By IFrame => By.Id("ctl00_ctl00_ctl00_TopLoginIFrame1_iFrameContent2");
        }

        public List<HarrisCriminalStyleDto> GetData(IWebDriver driver, IEnumerable<string> caseNumbers)
        {
            var result = new List<HarrisCriminalStyleDto>();
            if (caseNumbers == null) return result;
            var list = caseNumbers.Distinct().ToList();
            foreach (var caseNumber in list)
            {
                if (list.IndexOf(caseNumber) == 1)
                {
                    System.Diagnostics.Debugger.Break();
                }
                result.Append(GetData(driver, caseNumber));
            }
            return result;
        }

        public List<HarrisCriminalStyleDto> GetData(IWebDriver driver, string caseNumber)
        {
            var result = new List<HarrisCriminalStyleDto>();
            if (driver == null)
            {
                driver = GetDriver();
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
                if (index == 1)
                {
                    // System.Diagnostics.Debugger.Break();
                }
                result.Add(dto);
            }
            return result;
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
