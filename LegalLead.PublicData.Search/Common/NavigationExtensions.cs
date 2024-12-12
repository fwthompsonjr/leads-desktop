using HtmlAgilityPack;
using LegalLead.PublicData.Search.Classes;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Common
{
    internal static class NavigationExtensions
    {
        public static void WaitForDocumentReady(
            this IWebDriver driver,
            IJavaScriptExecutor jsexec,
            TimeSpan tswait,
            TimeSpan tsPoolingInterval)
        {
            const string request = "return document.readyState";
            const string response = "complete";
            var wait = new WebDriverWait(driver, tswait) { PollingInterval = tsPoolingInterval };
            wait.Until(driver1 =>
            {
                var obj = jsexec.ExecuteScript(request);
                if (obj is not string pagestatus) return false;
                return pagestatus.Equals(response);
            });
        }

        public static HtmlDocument GetHtml(this IWebDriver driver, By locator, string propertyName)
        {
            var element = driver.TryFindElement(locator);
            if (element == null) return null;
            var html = element.GetAttribute(propertyName);
            if (string.IsNullOrEmpty(html)) return null;
            var arr = new List<string>
                {
                    "<html>",
                    "<body>",
                    html,
                    "</body>",
                    "</html>"
                };
            var content = string.Join(Environment.NewLine, arr);
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            return doc;
        }

        public static IJavaScriptExecutor GetJsExecutor(
            this IWebDriver driver)
        {
            if (driver is IJavaScriptExecutor exec) return exec;
            return null;
        }

        public static DallasSearchProcess ToDallasSearch(this TravisSearchProcess source)
        {
            if (source == null) return null;
            var result = new DallasSearchProcess();
            result.SetSearchParameters(source.StartDate.ToNullableDate(), source.EndingDate.ToNullableDate(), source.CourtType);
            return result;
        }

        public static DateTime? ToNullableDate(this string value)
        {
            if (!string.IsNullOrEmpty(value)) return null;
            if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var date)) return date;
            return null;
        }


    }
}
