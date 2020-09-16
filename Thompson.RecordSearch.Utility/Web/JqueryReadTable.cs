using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;

namespace Thompson.RecordSearch.Utility.Web
{


    using Byy = OpenQA.Selenium.By;
    using Thompson.RecordSearch.Utility.Dto;
    using OpenQA.Selenium;
    using System.Threading;
    using Thompson.RecordSearch.Utility.Models;

    public class JqueryReadTable : ElementActionBase
    {
        private static Dictionary<string, string> _scripts;

        protected static string GetDataScript(string suffix = "")
        {
            const string dataFormat = @"{0}\xml\{1}.js";
            if(_scripts == null)
            {
                _scripts = new Dictionary<string, string>();
            }
            if (_scripts.ContainsKey(suffix))
            {
                return _scripts[suffix];
            }
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                suffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException(
                    CommonKeyIndexes.NavigationFileNotFound);
            }
            var data = File.ReadAllText(dataFile);
            _scripts.Add(suffix, data);
            return data;
        }


        const string actionName = "jquery-read-table";
        const string rowSelector = "#itemPlaceholderContainer tr.even";
        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
            if (item == null) throw new ArgumentNullException(nameof(item));
            var driver = GetWeb;
            var selector = item.Locator.Query;
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            if (string.IsNullOrEmpty(selector)) return;
            var command = GetDataScript(selector);
            List<CaseRowData> rowData = new List<CaseRowData>();
            var rows = driver.FindElements(Byy.CssSelector(rowSelector)).ToList();
            rows.ForEach(row => 
                {
                    var caseData = new CaseRowData();
                    var cells = row.FindElements(Byy.TagName("td")).ToList();
                    for (var i = 1; i <= 8; i++)
                    {
                        var cell = cells[i];
                        switch (i)
                        {
                            case 1:
                                caseData.Case = cell.FindElement(Byy.CssSelector("a.doclinks")).Text;
                                break;
                            case 2:
                                caseData.Court = cell.Text.Trim();
                                break;
                            case 3:
                                caseData.FileDate = cell.Text.Trim();
                                break;
                            case 4:
                                caseData.Status = cell.Text.Trim();
                                break;
                            case 5:
                                caseData.TypeDesc = cell.Text.Trim();
                                break;
                            case 6:
                                caseData.Subtype = cell.Text.Trim();
                                break;
                            case 7:
                                caseData.Style = cell.Text.Trim();
                                break;
                            case 8:
                                var hlink = cell.FindElement(Byy.TagName("a"));
                                var link = hlink.GetAttribute("onclick");
                                var n = link.IndexOf("x-", comparisonType: ccic) + 1;
                                link = link.Substring(n);
                                n = link.IndexOf(".", comparisonType: ccic);
                                link = link.Substring(1, n - 1);
                                executor.ExecuteScript("arguments[0].click();", hlink);
                                caseData.CaseDataAddresses = GetAddresses(link);
                                rowData.Add(caseData);
                                executor.ExecuteScript("arguments[0].click();", hlink);
                                break;
                        }
                    }
                });

            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }


        private List<CaseDataAddress> GetAddresses(string search)
        {
            var data = new List<CaseDataAddress>();
            var driver = GetWeb;
            var div = driver.FindElement(Byy.CssSelector("div[id*='" + search + "']"));
            var elements = div.FindElements(Byy.CssSelector("table[rules='rows'] tr[align='center']")).ToList();

            for (var i = 0; i < elements.Count; i++)
            {
                var row = elements[i];
                var tds = row.FindElements(Byy.TagName("td")).ToList();
                var obj = new CaseDataAddress
                {
                    Case = HtmlDecode(tds[0].Text.Trim()),
                    Role = HtmlDecode(tds[1].Text.Trim()),
                    Party = HtmlDecode(tds[2].GetAttribute("innerHTML").Trim()),
                    Attorney = HtmlDecode(tds[3].GetAttribute("innerHTML").Trim())
                };
                data.Add(obj);

            }
            return data;
        }

        private string HtmlDecode(string input)
        {
            const string pipe = " | ";
            var cleaned = System.Net.WebUtility.HtmlDecode(input);
            var sb = new StringBuilder(cleaned);
            sb.Replace("<br>", pipe);
            sb.Replace("<br/>", pipe);
            sb.Replace("<br />", pipe);
            return sb.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", 
            "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected bool HasNextPage()
        {
            try
            {
                var driver = GetWeb;
                driver.FindElement(Byy.CssSelector(""));
                return true;
            }
            catch (Exception)
            {
                // element not found or something
                return false;
            }
        }
    }
}
