using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasFetchCaseItems : DallasFetchCaseDetail
    {
        public override int OrderId => 55;
        public bool PauseForPage { get; set; }
        public override object Execute()
        {
            const string elementId = "CasesGrid";
            var columns = new List<string> {
                "party-case-caseid",
                "party-case-filedate",
                "party-case-type",
                "party-case-status",
                "party-case-location",
                "party-case-partyname" };
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            if (PauseForPage) Thread.Sleep(2000);
            var alldata = new List<DallasCaseItemDto>();
            var locator = By.Id(elementId);
            var element = Driver.FindElement(locator);
            var content = element.GetAttribute("outerHTML");
            var doc = GetHtml(content);
            var node = doc.DocumentNode;
            var links = node.SelectNodes("//a").ToList().FindAll(a =>
            {
                var attr = a.Attributes.FirstOrDefault(aa => aa.Name == "class");
                if (attr == null) return false;
                return attr.Value == "caseLink";
            });
            links.ForEach(lnk =>
            {
                var linkurl = lnk.GetAttributeValue("data-url", "");
                var parentRow = GetClosest("tr", lnk);
                if (parentRow != null)
                {
                    var datarow = parentRow.SelectNodes("td").ToList().FindAll(d =>
                    {
                        var attr = d.Attributes.FirstOrDefault(aa => aa.Name == "class");
                        if (attr == null) return false;
                        var found = false;
                        var classlist = attr.Value;
                        columns.ForEach((c) => { if (classlist.Contains(c)) { found = true; } });
                        return found;
                    });
                    var data = new DallasCaseItemDto
                    {
                        Href = linkurl,
                        CaseNumber = datarow[0].InnerText,
                        FileDate = datarow[1].InnerText,
                        CaseType = datarow[2].InnerText,
                        CaseStatus = datarow[3].InnerText,
                        Court = datarow[4].InnerText,
                        PartyName = datarow[5].InnerText
                    };
                    if (data.CaseStatus == "OPEN") { alldata.Add(data); }
                }
            });
            Console.WriteLine("Search found {0} records", alldata.Count);
            return JsonConvert.SerializeObject(alldata);
        }

        protected static HtmlNode GetClosest(string tagName, HtmlNode element)
        {
            const StringComparison Oic = StringComparison.OrdinalIgnoreCase;
            if (element == null) return null;
            var parent = element.ParentNode;
            while(!parent.Name.Equals(tagName, Oic)) parent = parent.ParentNode;
            return parent;
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

        protected override string ScriptName { get; } = "get case list";
    }
}