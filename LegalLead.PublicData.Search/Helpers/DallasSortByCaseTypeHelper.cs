﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Helpers
{
    public class DallasSortByCaseTypeHelper
    {
        private readonly IWebDriver Driver;
        private readonly IJavaScriptExecutor JsExecutor;
        public DallasSortByCaseTypeHelper(
            IWebDriver driver,
            IJavaScriptExecutor executor)
        {
            Driver = driver;
            JsExecutor = executor;
        }

        public void Execute()
        {
            WaitForSelector();
            var retries = 5;
            while (!IsSorted())
            {
                Sort();
                retries--;
                if (retries == 0) break;
                Thread.Sleep(150);
            }
        }

        private bool IsSorted()
        {
            var content = string.Join(Environment.NewLine, sortscript);
            const string command = "return typeSort.isSorted();";
            var js = string.Concat(content, Environment.NewLine, command);
            var response = JsExecutor.ExecuteScript(js);
            if (response is not bool isSorted) return false;
            return isSorted;
        }

        private void Sort()
        {
            var content = string.Join(Environment.NewLine, sortscript);
            const string command = "return typeSort.click();";
            var js = string.Concat(content, Environment.NewLine, command);
            JsExecutor.ExecuteScript(js);
        }

        private void WaitForSelector()
        {
            try
            {

                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30)) { PollingInterval = TimeSpan.FromMilliseconds(400) };
                wait.Until(w =>
                {
                    var collection = w.TryFindElements(By.XPath(_sortLink));
                    if (collection == null) return false;
                    return collection.Any(x => x.Text == "Type");
                });
            }
            catch
            {
                Debug.WriteLine("Failed to find selector");
            }
        }

        private static readonly string[] sortscript = new[]
        {
            "var typeSort = { ",
            "	'getElement': function() { ",
            "		var arr = Array.prototype.slice.call( document.getElementsByTagName('a'), 0) ",
            "		.filter(x => { let attr = x.getAttribute('class'); return attr != null && attr == 'k-link'}) ",
            "		.filter(x => x.innerText == 'Type'); ",
            "		if (arr != null && arr.length > 0 ) return arr[0]; ",
            "		return null; ",
            "	}, ",
            "	isSorted: function(lnk) { ",
            "",
            "		if (undefined == lnk || null == lnk || !lnk ) { lnk = typeSort.getElement(); } ",
            "		var spns = lnk.getElementsByTagName('span'); ",
            "		if (null == spns || spns.length == 0) { return false; } ",
            "		var attr = spns[0].getAttribute('class'); ",
            "		if (attr == null || attr.indexOf('k-i-arrow-n') == -1) { return false; } ",
            "		return true; ",
            "	}, ",
            "	click: function() { ",
            "		var ele = typeSort.getElement(); ",
            "		if (ele == null) { return; } ",
            "		ele.click();  ",
            "	} ",
            "} "
        };
        private const string _sortLink = "//a[@class = 'k-link']";
    }
}