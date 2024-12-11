using LegalLead.PublicData.Search.Common;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Helpers
{
    internal class DallasJusticeHelper
    {
        public DallasJusticeHelper(
            IWebDriver web,
            IJavaScriptExecutor javaScript)
        {
            driver = web;
            engine = javaScript;
        }

        public List<DallasJusticeOfficer> GetOfficers()
        {
            const char comma = ',';
            if (JusticeOfficers.Count > 0) return JusticeOfficers;
            var list = new List<DallasJusticeOfficer>();
            if (driver == null) return list;
            if (engine == null) return list;
            if (!Uri.TryCreate(NavAddress, UriKind.Absolute, out var uri)) return list;
            driver.Navigate().GoToUrl(uri);
            var content = engine.ExecuteScript(JsContent);
            if (content is not string js) return list;
            var arr = JsonConvert.DeserializeObject<List<string>>(js);
            if (arr == null || arr.Count == 0) return list;
            arr.ForEach(a =>
            {
                if (a.Contains(comma))
                {
                    var items = a.Split(comma);
                    list.Add(new()
                    {
                        Name = items[0].Trim(),
                        Court = items[^1].Trim()
                    });
                }
            });
            JusticeOfficers.AddRange(list);
            return JusticeOfficers;
        }
        private static string JsContent
        {
            get
            {
                if (!string.IsNullOrEmpty(jscontent)) return jscontent;
                jscontent = string.Join(Environment.NewLine, blockjs);
                return jscontent;
            }
        }
        private static string jscontent = null;
        private static string[] blockjs = new string[]
        {
            "var js_justice = { ",
            "'get_officers': function() ",
            "{ ",
            "	var ofclist = []; ",
            "	try { ",
            "	var dvfind = 'dc-sidebar-collapsible dc_accordian'; ",
            "	var linkfind = 'government/jpcourts'; ",
            "	var dvaccordian = Array.prototype.slice.call( ",
            "	document.getElementsByTagName('div'), 0) ",
            "	.find(a => { ",
            "		let cls = a.getAttribute('class'); ",
            "		return cls != null && cls == dvfind; ",
            "	}); ",
            "	var links = dvaccordian.innerText.split('\n').filter(d => { return d.indexOf(',') > 0 }); ",
            "	links.forEach(tx => ofclist.push(tx)); ",
            "	return JSON.stringify(ofclist); ",
            "	} catch { ",
            "	return JSON.stringify(ofclist); ",
            "	} ",
            "} ",
            "}; "
        };
        private readonly IWebDriver driver;
        private readonly IJavaScriptExecutor engine;
        private static readonly List<DallasJusticeOfficer> JusticeOfficers = new List<DallasJusticeOfficer>();
        private const string NavAddress = "https://www.dallascounty.org/government/jpcourts/";
    }
}
