using HtmlAgilityPack;
using LegalLead.PublicData.Search.Common;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class FortBendFetchCaseList : BaseFortBendSearchAction
    {
        public override int OrderId => 55;
        public override object Execute()
        {

            if (Parameters == null || Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            var locator = By.XPath("//table[@border='0'][@cellpadding='2']");
            WaitForTable(locator);

            var doc = Driver.GetHtml(locator, "outerHTML");
            var alldata = new List<CaseItemDto>();
            var node = doc.DocumentNode;
            var links = node.SelectNodes("//tr").ToList().FindAll(a =>
            {
                var tx = a.InnerHtml;
                return !tx.Contains("<th");
            });
            links.ForEach(lnk =>
            {
                var itm = GetRowItem(lnk);
                if (itm != null && !string.IsNullOrEmpty(itm.Href)) alldata.Add(itm);
            });

            if (!string.IsNullOrEmpty(RecordFoundMesage))
                Console.WriteLine(RecordFoundMesage, alldata.Count);

            return JsonConvert.SerializeObject(alldata);
        }

        protected void WaitForTable(By locator)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10)) { PollingInterval = TimeSpan.FromMilliseconds(500) };
                wait.Until(w =>
                {
                    var item = w.TryFindElement(locator);
                    return item != null;
                });
            }
            catch (Exception)
            {
                Debug.WriteLine("Wait for table object experienced error.");
            }
        }

        protected static string GetDivText(HtmlNode element, int divId)
        {
            var divs = element.SelectNodes("div")?.ToList();
            if (divs == null || divs.Count - 1 < divId) return string.Empty;
            return divs[divId].InnerText.Trim();
        }

        protected static string GetLinkAddress(HtmlNode element)
        {
            const string question = "?";
            const string linkformat = "https://tylerpaw.fortbendcountytx.gov/PublicAccess/CaseDetail.aspx?CaseID={0}";
            var cell = element.SelectSingleNode("a");
            if (cell == null) return string.Empty;
            var attr = cell.Attributes.FirstOrDefault(aa => aa.Name == "href");
            if (attr == null || !attr.Value.Contains(question)) return string.Empty;
            var subset = attr.Value.Split('?');
            var detail = subset[^1];
            var indx = detail.Split('=')[^1];
            return string.Format(CultureInfo.CurrentCulture, linkformat, indx);
        }

        protected static CaseItemDto GetRowItem(HtmlNode element)
        {
            var data = new CaseItemDto();
            var cells = element.SelectNodes("td").ToList();
            if (cells.Count < 5) return null;
            data.Href = GetLinkAddress(cells[0]);
            data.CaseNumber = cells[0].InnerText.Trim();
            data.CaseStyle = cells[2].InnerText.Trim();
            data.FileDate = GetDivText(cells[3], 0);
            data.Court = GetDivText(cells[3], 1);
            data.CaseType = GetDivText(cells[4], 0);
            data.CaseStatus = GetDivText(cells[4], 1);
            data.SetPartyNameFromCaseStyle(true);
            return data;
        }

        private static string recordFoundMessage;
        protected static string RecordFoundMesage
        {
            get
            {
                return recordFoundMessage ??= AppMessages.GetMessage("STATUS_MSG_SEARCH_FOUND_RECORDS");
            }
        }
    }
}