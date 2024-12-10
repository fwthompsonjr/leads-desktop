using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Helpers
{
    internal class TarrantAddressHelper
    {
        private readonly IWebDriver driver;
        private readonly IJavaScriptExecutor jsExec;
        private readonly TarrantWebInteractive twInteractive;
        private readonly HLinkDataRow dataRow;
        public TarrantAddressHelper(
            IWebDriver theDriver,
            TarrantWebInteractive interactive,
            HLinkDataRow linkData)
        {
            driver = theDriver;
            twInteractive = interactive;
            dataRow = linkData;
            jsExec = (IJavaScriptExecutor)theDriver;
        }

        public bool GetAddressInformation()
        {
            var fmt = twInteractive.GetParameterValue<string>(CommonKeyIndexes.HlinkUri);
            var xpath = twInteractive.GetParameterValue<string>(CommonKeyIndexes.PersonNodeXpath);
            var condition = By.XPath(xpath);
            var helper = new ElementAssertion(driver);
            helper.Navigate(string.Format(CultureInfo.CurrentCulture, fmt, dataRow.WebAddress));
            var tdName = WaitForElement(condition);
            if (tdName == null) return false;
            var body = jsExec.ExecuteScript("return document.getElementsByTagName('body')[0].innerHTML");
            if (!(body is string jsbody)) return false;

            /* 
             * Notes:
             * 1. use js-exec to fetch row detail in single statement
             * 2. return
             * - rowIndex
             * - td - inner text
             * - closest table inner html
             */
            var data = new[]
            {
                "<html>",
                "<body>",
                jsbody,
                "</body>",
                "</html>"
            };
            var html = string.Join(Environment.NewLine, data);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodeName = doc.DocumentNode.SelectSingleNode(xpath);
            if (nodeName == null) return false;
            return true;
        }
        private IWebElement WaitForElement(By condition)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500)
                };
                wait.Until(d =>
                {
                    return d.TryFindElement(condition) != null;
                });
                return driver.TryFindElement(condition);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
