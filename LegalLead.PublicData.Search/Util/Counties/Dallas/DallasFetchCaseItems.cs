﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;
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
            const string noElementId = "ui-tabs-1";
            var columns = new List<string> {
                "party-case-caseid",
                "party-case-filedate",
                "party-case-type",
                "party-case-status",
                "party-case-location",
                "party-case-partyname" };

            var executor = GetJavaScriptExecutor();
            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            TryHideElements(executor);
            var alldata = new List<CaseItemDto>();
            try
            {

                var locator1 = By.Id(noElementId);
                WaitForElement(locator1);
                if (IsNoCount(executor)) return string.Empty;
                var locator = By.Id(elementId);
                WaitForElement(locator);
                var element = TryGetElement(Driver, locator);
                if (element == null) return string.Empty;

                // run script to reveal max-rows

                var content = element.GetAttribute("outerHTML");
                var doc = GetHtml(content);
                var node = doc.DocumentNode;
                var links = node.SelectNodes("//a").ToList().FindAll(a =>
                {
                    var attr = a.Attributes.FirstOrDefault(aa => aa.Name == "class");
                    if (attr == null) return false;
                    return attr.Value == "caseLink";
                });

                if (links == null || links.Count == 0)
                    return JsonConvert.SerializeObject(alldata);

                var mx = links.Count;
                links.ForEach(lnk =>
                {
                    Console.WriteLine($"Fetching address detail: {links.IndexOf(lnk) + 1} of {mx}");
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
                        var data = new CaseItemDto
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
            catch (Exception)
            {
                return JsonConvert.SerializeObject(alldata);
            }
        }

        private void WaitForElement(By locator)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500)
                };
                wait.Until(d =>
                {
                    var element = d.TryFindElement(locator);
                    return element != null;
                });
            }
            catch (Exception)
            {
                // suppress errors
            }
        }

        protected static HtmlNode GetClosest(string tagName, HtmlNode element)
        {
            const StringComparison Oic = StringComparison.OrdinalIgnoreCase;
            if (element == null) return null;
            var parent = element.ParentNode;
            while (!parent.Name.Equals(tagName, Oic)) parent = parent.ParentNode;
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
        /// <summary>
        /// Tries the find element on a specfic web page using the By condition supplied.
        /// </summary>
        /// <param name="parent">The parent web browser instance.</param>
        /// <param name="by">The by condition used to locate the element</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private static IWebElement TryGetElement(IWebDriver parent, By by)
        {
            try
            {
                if (parent == null) return null;
                return parent.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}