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
    public class TravisFetchClickStyle : BaseTravisSearchAction
    {
        public override int OrderId => 65;
        public bool PauseForPage { get; set; }
        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            if (PauseForPage) Thread.Sleep(2000);
            var alldata = new List<TravisCaseStyleDto>();
            var currentUri = Driver.Url;

            if (!Uri.TryCreate(currentUri, UriKind.Absolute, out var uri))
                throw new ArgumentException(Rx.ERR_URI_MISSING);
            var collection = GetLinks();
            if (collection == null || collection.Count == 0)
                return JsonConvert.SerializeObject(alldata);
            var id = 0;
            var mx = collection.Count;
            var retries = new List<int>();
            while (id < mx)
            {
                var current = id;
                var element = GetLink(id++);
                element.Click();
                
                Thread.Sleep(500);
                
                var body = Driver.FindElement(By.TagName("body"));
                var content = body.GetAttribute("innerHTML");
                var dto = GetDto(content);
                if (dto != null) 
                    alldata.Add(dto);
                else
                    retries.Add(current);

                Driver.Navigate().Back();
            }
            
            Console.WriteLine("Search mapped {0} records", alldata.Count);
            return JsonConvert.SerializeObject(alldata);
        }

        private List<IWebElement> GetLinks()
        {
            const string linkIndicator = "CaseDetail.aspx?";
            const string styleIndicator = "color: blue";
            return Driver.FindElements(By.TagName("a"))
                .Where(a =>
                {
                    var navigationTo = a.GetAttribute("href");
                    if (string.IsNullOrEmpty(navigationTo)) return false;
                    var idx = navigationTo.IndexOf(linkIndicator, StringComparison.OrdinalIgnoreCase);
                    return idx >= 0;
                })
                .Where(a =>
                {
                    var styleBlock = a.GetAttribute("style");
                    if (string.IsNullOrEmpty(styleBlock)) return false;
                    var idx = styleBlock.IndexOf(styleIndicator, StringComparison.OrdinalIgnoreCase);
                    return idx >= 0;
                })
                .ToList();
        }

        private IWebElement GetLink(int index)
        {
            if (index < 0) return null;
            var list = GetLinks();
            if (index > list.Count - 1) return null;
            var item = list[index];
            if (!(Driver is IJavaScriptExecutor executor)) return item;
            executor.ExecuteScript("arguments[0].scrollIntoView(true);", item);
            return item;
        }

        private static TravisCaseStyleDto GetDto(string pageHtml)
        {

            const string nospace = "&nbsp;";
            const string linbreak = "<br>";
            const string twopipe = "||";
            const string pipe = "|";
            const string space = " ";
            StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (string.IsNullOrEmpty(pageHtml)) return null;
            var doc = GetHtml(pageHtml);
            var node = doc.DocumentNode;
            var obj = new TravisCaseStyleDto();
            var dv = node.SelectNodes("//div").ToList().Find(x =>
            {
                var attr = x.Attributes.FirstOrDefault(b => b.Name == "class");
                if (attr == null) { return false; }
                return attr.Value.Equals("ssCaseDetailCaseNbr", comparison);
            });
            var tables = node.SelectNodes("//table").ToList();
            var headers = node.SelectNodes("//th").ToList();
            var courtName = headers.Find(x =>
            {
                if (string.IsNullOrEmpty(x.InnerText)) return false;
                return x.InnerText.Equals("Location:", comparison);
            });
            headers = headers.FindAll(a =>
            {
                var attr = a.Attributes.FirstOrDefault(b => b.Name == "rowspan");
                if (attr == null) { return false; }
                return attr.Value.Equals("2", comparison);
            });
            if (tables.Count < 3) return null;
            if (headers.Count < 2) return null;
            if (dv != null)
            {
                obj.CaseNumber = dv.InnerText.Split('.')[1].Trim();
            }
            var plantId = headers.FindIndex(x => x.InnerText.Equals("Plaintiff", comparison));
            if (plantId < 0) plantId = headers.Count - 1;

            
            var ndeCourt = courtName?.ParentNode;
            while (ndeCourt != null && !ndeCourt.Name.Equals("tr", comparison)) ndeCourt = ndeCourt.ParentNode;
            if (ndeCourt != null)
            {
                obj.Court = ndeCourt.ChildNodes[1].InnerText.Trim();
            }
            obj.CaseStyle = tables[4].SelectNodes("//b")[0].InnerText;
            obj.PartyName = headers[0].ParentNode.ChildNodes[1].InnerText;
            obj.Plaintiff = headers[plantId].ParentNode.ChildNodes[1].InnerText;
            var ndeParty = headers[0].ParentNode;
            var tbl = ndeParty.ParentNode;
            while (!tbl.Name.Equals("table", comparison)) tbl = tbl.ParentNode;
            while (!ndeParty.Name.Equals("tr", comparison)) ndeParty = ndeParty.ParentNode;
            var rwindex = ndeParty.GetAttributeValue("rowIndex", 1);
            var tbody = tbl.ChildNodes.ToList().Find(x => x.Name.Equals("tbody", comparison));
            var rw = tbody.ChildNodes[rwindex + 1];
            var addr = rw.SelectNodes("td")[0].InnerHtml.Trim();
            while (addr.IndexOf(nospace, comparison) >= 0) { addr = addr.Replace(nospace, space); }
            while (addr.IndexOf(linbreak, comparison) >= 0) { addr = addr.Replace(linbreak, pipe); }
            while (addr.IndexOf(twopipe, comparison) >= 0) { addr = addr.Replace(twopipe, pipe); }
            addr = addr.Trim();
            if (addr.EndsWith(pipe, comparison)) { addr = addr.Substring(0, addr.Length - 1); }
            if (addr.IndexOf(pipe, comparison) < 0 && addr.Length > 0) { addr = string.Concat("000 No Street Address|", addr); }
            obj.Address = addr;
            return obj;

        }
        private static HtmlDocument GetHtml(string html)
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
    }
}