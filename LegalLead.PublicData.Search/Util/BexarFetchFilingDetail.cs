﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarFetchFilingDetail : BaseBexarSearchAction
    {
        public override int OrderId => 70;
        public override object Execute()
        {
            if (Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            var alldata = new List<CaseItemDto>();
            var collection = GetLinks();
            if (collection == null || collection.Count == 0)
                return JsonConvert.SerializeObject(alldata);

            var id = 0;
            var mx = collection.Count;
            while (id < mx)
            {
                var element = GetLink(id++);
                element.Click();
                Thread.Sleep(500);

                var body = Driver.FindElement(By.TagName("body"));
                var content = body.GetAttribute("innerHTML");
                var dto = GetDto(content);
                if (dto != null) alldata.Add(dto);
            }
            return JsonConvert.SerializeObject(alldata);
        }
        private List<IWebElement> GetLinks()
        {
            const string linkIndicator = "RegisterOfActions";
            var elements = Driver.FindElements(By.TagName("a"));
            return elements.Where(a =>
                {
                    var navigationTo = a.GetAttribute("data-url");
                    if (string.IsNullOrEmpty(navigationTo)) return false;
                    var idx = navigationTo.Contains(linkIndicator, StringComparison.OrdinalIgnoreCase);
                    return idx;
                })
                .ToList();
        }

        private IWebElement GetLink(int index)
        {
            if (index < 0) return null;
            var list = GetLinks();
            if (index > list.Count - 1) return null;
            var item = list[index];
            if (Driver is not IJavaScriptExecutor executor) return item;
            executor.ExecuteScript("arguments[0].scrollIntoView(true);", item);
            return item;
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
    }
}