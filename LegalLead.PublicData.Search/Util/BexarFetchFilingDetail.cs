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
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarFetchFilingDetail : BaseBexarSearchAction
    {
        public override int OrderId => 70;
        public bool IsTestMode { get; set; }
        public IJavaScriptExecutor ExternalExecutor { get; set; } = null;

        public override object Execute()
        {
            string caseLocator = "//div[@id = 'roa-header']";
            const string itemLocator = "//a[@class='caseLink show-only-in-desktop-view']";
            if (Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            var alldata = new List<CaseItemDto>();
            var collection = this.GetCaseNumbers(itemLocator, CustomLinkJs);
            if (collection == null || collection.Count == 0)
                return JsonConvert.SerializeObject(alldata);

            var id = 0;
            var mx = collection.Count;
            while (id < mx)
            {
                Console.WriteLine("Reading item: {0} of {1}", id, mx);
                var itemscript = CustomClickJs.Replace("~0", id.ToString());
                this.ClickCaseNumber(collection[id], id++, caseLocator, itemscript);
                Driver.SwitchTo().Window(Driver.WindowHandles[^1]);
                var dto = TryFetchDto();
                if (dto != null)
                {
                    var helper = new BexarFetchFilingHelper { Driver = Driver };
                    var info = helper.GetAddress();
                    if (info != null)
                    {
                        dto.PartyName = info.PartyName;
                        dto.Address = info.Address;
                        dto.Court = info.Court;
                    }
                    alldata.Add(dto);
                }
                if (Driver.WindowHandles.Count > 1)
                {
                    Driver.Close();
                    Driver.SwitchTo().Window(Driver.WindowHandles[0]);
                }
            }
            return JsonConvert.SerializeObject(alldata);
        }

        private CaseItemDto TryFetchDto()
        {
            try
            {
                CaseItemDto dto = null;
                WaitForElementsExist();
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(90)) { PollingInterval = TimeSpan.FromMilliseconds(750) };
                wait.Until(w =>
                {
                    var exec = Driver.GetJsExecutor();
                    var body = exec.ExecuteScript(GetBodyJs);
                    if (body is not string content) return false;
                    if (string.IsNullOrEmpty(content)) return false;
                    dto = GetDto(content);
                    return dto != null;
                });
                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void WaitForElementsExist()
        {
            try
            {
                if (IsTestMode) return;
                var selectors = new[] {
                    By.TagName("body"),
                    By.Id("header-info"),
                    By.Id("party-info"),
                    By.XPath("//tr[@class='roa-party-row roa-pad-b-10 ng-scope']") }.ToList();
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30)) { PollingInterval = TimeSpan.FromSeconds(1) };
                wait.Until(d =>
                {
                    var exists = false;
                    selectors.ForEach(sel =>
                    {
                        var item = d.TryFindElement(sel);
                        exists = item != null;
                    });
                    return exists;
                });
            }
            catch
            {
                Debug.WriteLine("WaitForElementsExist failed to find collection");
            }
        }


        private static CaseItemDto GetDto(string pageHtml)
        {
            const string dvStyleName = "flex-50";
            if (string.IsNullOrEmpty(pageHtml)) return null;
            var doc = GetHtml(pageHtml);
            if (doc == null) return null;
            var node = doc.DocumentNode;
            var dv = node.SelectNodes("//div").ToList().FindAll(x =>
            {
                var attr = x.Attributes.FirstOrDefault(b => b.Name == "class");
                if (attr == null) { return false; }
                return attr.Value.Contains(dvStyleName, comparison);
            });
            if (dv == null || dv.Count != 2) return null;
            var party = GetNameFromCaseStyle(dv[0].InnerText.Trim());
            var filingDt = GetFilingDate(dv[1]);
            var caseNo = GetCaseNumber(node);
            return new()
            {
                CaseNumber = caseNo,
                PartyName = party,
                FileDate = filingDt,
            };
        }

        private static HtmlDocument GetHtml(string html)
        {
            try
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
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetNameFromCaseStyle(string caseStyle)
        {
            const string find = "VS ";
            const string etal = "ET AL";
            const char space = ' ';
            if (string.IsNullOrEmpty(caseStyle)) return string.Empty;
            if (!caseStyle.Contains(find)) return string.Empty;
            var indx = caseStyle.IndexOf(find);
            var name = indx == -1 ? string.Empty : caseStyle.Substring(indx + find.Length);
            if (name.EndsWith(etal))
            {
                name = name.Substring(0, name.Length - etal.Length).Trim();
            }
            if (name.Contains(space))
            {
                var names = name.Split(space).ToList();
                if (names.Count == 2)
                {
                    name = $"{names[1]}, {names[0]}";
                }
                if (names.Count > 2)
                {
                    var ending = string.Join(" ", names.GetRange(1, names.Count - 1));
                    name = $"{ending}, {names[0]}";
                }
            }
            return name;
        }

        private static string GetFilingDate(HtmlNode node)
        {
            const char colon = ':';
            var dv = node.SelectNodes("div").ToList().Find(x =>
            {
                var attr = x.Attributes.FirstOrDefault(b => b.Name == "label");
                if (attr == null) { return false; }
                return attr.Value.Contains("Filed on", comparison);
            });
            if (dv == null) return string.Empty;
            if (!dv.HasChildNodes || dv.ChildNodes.Count < 2) return string.Empty;
            var text = dv.ChildNodes[1].InnerText.Trim();
            if (!text.Contains(colon)) return string.Empty;
            var dte = text.Split(colon)[^1].Trim();
            if (!DateTime.TryParse(dte, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var _)) return string.Empty;
            return dte;
        }
        private static string GetCaseNumber(HtmlNode node)
        {
            const char space = ' ';
            var dv = node.SelectNodes("//div").ToList().Find(x =>
            {
                var attr = x.Attributes.FirstOrDefault(b => b.Name == "id");
                if (attr == null || string.IsNullOrEmpty(attr.Value)) { return false; }
                return attr.Value.Equals("roa-header", comparison);
            });
            if (dv == null) return string.Empty;
            var children = dv.SelectNodes("div")?.ToList();
            if (children == null || children.Count < 3) return string.Empty;
            var text = children[2].InnerText.Trim();
            if (!text.Contains(space)) return string.Empty;
            return text.Split(space)[^1].Trim();
        }

        private const StringComparison comparison = StringComparison.OrdinalIgnoreCase;

        private static readonly string[] getlinkjs = new string[]
        {
            "var links = Array.prototype.slice.call( document.getElementsByTagName('a'), 0 );",
            "links = links.filter(l => { let attr = l.getAttribute('class'); return attr != null && attr == 'caseLink show-only-in-desktop-view'; });",
            "var casenumbers = links.map(x => x.innerText.trim());",
            "return JSON.stringify(casenumbers);"
        };
        private static readonly string[] clicklinkjs = new string[]
        {
            "var indx = ~0",
            "var links = Array.prototype.slice.call( document.getElementsByTagName('a'), 0 );",
            "links = links.filter(l => { let attr = l.getAttribute('class'); return attr != null && attr == 'caseLink show-only-in-desktop-view'; });",
            "links[indx].click();"
        };

        private static readonly string[] getbodyjs = new string[]
        {
            "var bodies = document.getElementsByTagName('body')",
            "if (bodies.length < 1) return '';",
            "return bodies[0].innerHTML.trim();"
        };
        private static readonly string CustomLinkJs = string.Join(Environment.NewLine, getlinkjs);
        private static readonly string CustomClickJs = string.Join(Environment.NewLine, clicklinkjs);
        private static readonly string GetBodyJs = string.Join(Environment.NewLine, getbodyjs);
    }
}