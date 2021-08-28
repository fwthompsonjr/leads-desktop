using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.DriverFactory;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
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
            public static By UserName => By.Id(Controls.Text_UserName);
            public static By Password => By.Id(Controls.Text_Password);
            public static By Login => By.Id(Controls.Btn_Login);
            public static By CaseNumber => By.Id(Controls.Text_CaseNo);
            public static By Search => By.Id(Controls.Btn_Search);
            public static By Table => By.XPath(Controls.Table_Search);
            public static By TableRows => By.XPath(Controls.Table_Rows);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings", Justification = "<Pending>")]
        public List<HarrisCriminalStyleDto> GetData(IWebDriver driver, string caseNumber)
        {
            var result = new List<HarrisCriminalStyleDto>();
            if (driver == null)
            {
                driver = GetDriver();
            }
            driver.Navigate().GoToUrl(url);
            WaitForLoad(driver);
            Login(driver);

            if (!IsElementPresent(driver, Selectors.CaseNumber))
            {
                return result;
            }
            var txCaseNumber = driver.FindElement(Selectors.CaseNumber);
            ClickAndOrSetText(driver, txCaseNumber, caseNumber);
            if (!IsElementPresent(driver, Selectors.Search))
            {
                return result;
            }
            var btnSearch = driver.FindElement(Selectors.Search);
            ClickAndOrSetText(driver, btnSearch);
            if (!IsElementPresent(driver, Selectors.Table))
            {
                return result;
            }
            var rows = driver.FindElements(Selectors.Table).ToList();
            foreach (var item in rows)
            {
                var index = rows.IndexOf(item);
                if(index == 0) { continue; }
                var cells = item.FindElements(By.TagName("td"));
                var dto = new HarrisCriminalStyleDto
                {
                    Index = index,
                    CaseNumber = cells[0].Text,
                    Style = cells[0].Text,
                    FileDate = cells[0].Text,
                    Court = cells[0].Text,
                    Status = cells[0].Text,
                    TypeOfActionOrOffense = cells[0].Text
                };
                result.Add(dto);
            }
            return result;
        }

        [Obsolete]
        private static void WaitForLoad(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(ExpectedConditions.ElementToBeClickable(Selectors.Login));
        }

        private static void Login(IWebDriver driver)
        {
            if (!IsElementPresent(driver, Selectors.UserName, true) ||
                !IsElementPresent(driver, Selectors.Password, true) ||
                !IsElementPresent(driver, Selectors.Login, true))
            {
                return;
            }

            var txUserName = driver.FindElement(Selectors.UserName);
            var txPassword = driver.FindElement(Selectors.Password);
            var btnLogin = driver.FindElement(Selectors.Login);

            ClickAndOrSetText(driver, txUserName, uid);
            ClickAndOrSetText(driver, txPassword, pwd);
            ClickAndOrSetText(driver, btnLogin);
        }

        private static void ClickAndOrSetText(IWebDriver driver, IWebElement elementToClick, string objText = "")
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", elementToClick);
            if (!string.IsNullOrEmpty(objText))
            {
                executor.ExecuteScript(string.Format("arguments[0].value = '{0}';", objText), elementToClick);
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

        private static bool IsElementPresent(IWebDriver driver, By by, bool noWait)
        {
            try
            {
                if (!noWait)
                {
                    return IsElementPresent(driver, by);
                }
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
