using LegalLead.PublicData.Search.Common;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class ElPasoFetchCaseList : HidalgoFetchCaseList
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

            var mx = links.Count;
            var currentDate = Parameters.StartDate;
            links.ForEach(lnk =>
            {
                var indx = links.IndexOf(lnk);
                var message = $"Date: {currentDate} Reading item: {indx + 1} of {mx}";
                Interactive?.EchoProgess(0, mx, indx + 1, message, true);
                var itm = GetRowItem(lnk);
                if (IsValid(itm)) alldata.Add(itm);
            });
            Interactive?.CompleteProgess();
            if (!string.IsNullOrEmpty(RecordFoundMesage))
                Console.WriteLine(RecordFoundMesage, alldata.Count);

            return JsonConvert.SerializeObject(alldata);
        }

        private static bool IsValid(CaseItemDto itm)
        {
            if (itm == null) return false;
            if (string.IsNullOrEmpty(itm.Href)) return false;
            if (string.IsNullOrEmpty(itm.Court)) return false;
            if (itm.Court.Contains("County Criminal", StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }
    }
}