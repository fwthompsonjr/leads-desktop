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

    public class HarrisCivilReadTable : ElementActionBase
    {
        protected bool IsOverlaid { get; set; }
        const string actionName = "harris-civil-read-table";
        const string rowSelector = "#itemPlaceholderContainer tr.even";
        const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
        public override string ActionName => actionName;

        public List<HLinkDataRow> DataRows { get; private set; }
        public override void Act(NavigationStep item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var driver = GetWeb;
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            IsOverlaid = false;

            DataRows = new List<HLinkDataRow>();

            ReadPage(driver, executor);

            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }

        private void ReadPage(IWebDriver driver, IJavaScriptExecutor executor)
        {
            List<CaseRowData> rowData = new List<CaseRowData>();

            while (true)
            {
                if (HasNoData())
                {
                    var empty = new CaseRowData
                    {
                        FileDate = GetFileDateFromPage(),
                        CaseDataAddresses = new List<CaseDataAddress>()
                    {
                        new CaseDataAddress { Role = "Defendant" , Party = "No Data Returned" }
                    }
                    };
                    rowData.Add(empty);
                    break;
                }
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
                                var doclinks = cell.FindElement(Byy.CssSelector("a.doclinks"));
                                executor.ExecuteScript("arguments[0].scrollIntoView(true);", doclinks);
                                caseData.Case = doclinks.Text;
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
                if (!HasNextPage()) break;
                if (!NavigateNextPage())
                {
                    break;
                }
            }
            OuterHtml = rowData.ToHtml();
            rowData.ForEach(d => DataRows.AddRange(d.ConvertToDataRow()));


        }

        private bool NavigateNextPage()
        {
            var cssPagerLink = "a.pgr";
            var driver = GetWeb;
            // if there are paging hyperlinks then their might be a next page
            var paging = driver.FindElements(Byy.CssSelector(cssPagerLink));
            if (paging == null) return false;
            var list = paging.Cast<IWebElement>().ToList();
            var next = list.FirstOrDefault(a => a.Text.Contains("Next"));
            if (next == null) return false;
            if (!(next.GetAttribute("disabled") ?? "false").Equals("true", ccic))
            {
                // click element (executor)
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", next);
                return true;
            }
            return false;
        }

        private string GetFileDateFromPage()
        {
            const string dte = "MM/dd/yyyy";
            var cssSelector = "#ctl00_ContentPlaceHolder1_txtDateFrom";
            var found = GetWeb.TryFindElement(Byy.CssSelector(cssSelector));
            var dateMin = DateTime.MinValue.ToString(dte, CultureInfo.CurrentCulture);
            if (found == null)
            {
                return dateMin;
            }
            if(DateTime.TryParse(found.Text, out DateTime dtFrom)){
                return dtFrom.ToString(dte, CultureInfo.CurrentCulture);
            }
            return dateMin;
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

        private static string HtmlDecode(string input)
        {
            const string pipe = " | ";
            var cleaned = System.Net.WebUtility.HtmlDecode(input);
            var sb = new StringBuilder(cleaned);
            sb.Replace("<br>", pipe);
            sb.Replace("<br/>", pipe);
            sb.Replace("<br />", pipe);
            return sb.ToString();
        }


        protected bool HasNoData()
        {
            // #ctl00_ContentPlaceHolder1_lblListViewCasesEmptyMsg:visible
            var cssNoData = "ctl00_ContentPlaceHolder1_lblListViewCasesEmptyMsg";
            var driver = GetWeb;
            var found = driver.TryFindElement(Byy.Id(cssNoData));
            if (found == null) return false;
            return found.Displayed;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", 
            "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected bool HasNextPage()
        {
            try
            {
                var cssPagerLink = "a.pgr";
                var driver = GetWeb;
                // if there are paging hyperlinks then their might be a next page
                var paging = driver.FindElements(Byy.CssSelector(cssPagerLink));
                if (paging == null) return false;
                var list = paging.Cast<IWebElement>().ToList();
                var next = list.FirstOrDefault(a => a.Text.Contains("Next"));
                if (next == null) return false;
                return (next.GetAttribute("disabled") ?? "false").Equals("true", ccic);
            }
            catch (Exception)
            {
                // element not found or something
                return false;
            }
        }

    }
}
