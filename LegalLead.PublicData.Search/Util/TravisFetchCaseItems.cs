using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class TravisFetchCaseItems : BaseTravisSearchAction
    {
        public override int OrderId => 55;
        public bool PauseForPage { get; set; }
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            if (PauseForPage) Thread.Sleep(2000);


            js = VerifyScript(js);
            var table = executor.ExecuteScript(js);
            if (!(table is string tbhtml)) return string.Empty;
            var alldata = new List<CaseItemDto>();
            var doc = GetHtml(tbhtml);
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
            Console.WriteLine("Search found {0} records", alldata.Count);
            return JsonConvert.SerializeObject(alldata);
        }

        protected static HtmlDocument GetHtml(string html)
        {
            var arr = new List<string>
            {
                "<html>",
                "<body>",
                html,
                "</body>",
                "<</html>"
            };
            var content = string.Join(Environment.NewLine, arr);
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            return doc;
        }
        private static string GetDivText(HtmlNode element, int divId)
        {
            var divs = element.SelectNodes("div")?.ToList();
            if (divs == null || divs.Count - 1 < divId) return string.Empty;
            return divs[divId].InnerText.Trim();
        }

        private static string GetLinkAddress(HtmlNode element)
        {
            const string question = "?";
            const string linkformat = "https://odysseypa.traviscountytx.gov/JPPublicAccess/CaseDetail.aspx?{0}";
            var cell = element.SelectSingleNode("a");
            if (cell == null) return string.Empty;
            var attr = cell.Attributes.FirstOrDefault(aa => aa.Name == "href");
            if (attr == null || !attr.Value.Contains(question)) return string.Empty;
            var subset = attr.Value.Split('?');
            var detail = subset[subset.Length - 1];
            return string.Format(linkformat, detail);
        }

        private static CaseItemDto GetRowItem(HtmlNode element)
        {
            var data = new CaseItemDto();
            var cells = element.SelectNodes("td").ToList();
            if (cells.Count != 4) return null;
            data.Href = GetLinkAddress(cells[0]);
            data.CaseNumber = cells[0].InnerText.Trim();
            data.CaseStyle = cells[1].InnerText.Trim();
            data.FileDate = GetDivText(cells[2], 0);
            data.Court = GetDivText(cells[2], 1);
            data.CaseType = GetDivText(cells[3], 0);
            data.CaseStatus = GetDivText(cells[3], 1);
            return data;
        }

        protected override string ScriptName { get; } = "get case list";

    }
}