using LegalLead.PublicData.Search.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Helpers
{
    public class DallasSortByStatusHelper
    {
        protected readonly IWebDriver Driver;
        protected readonly IJavaScriptExecutor JsExecutor;
        public DallasSortByStatusHelper(
            IWebDriver driver,
            IJavaScriptExecutor executor)
        {
            Driver = driver;
            JsExecutor = executor;
        }

        public virtual void Execute()
        {
            if (NoCountHelper.IsNoCountData(JsExecutor)) return;
            if (!WaitForSelector()) return;
            var retries = 5;
            while (!IsSorted())
            {
                Sort();
                retries--;
                if (retries == 0) break;
                Thread.Sleep(150);
            }
            Filter();
        }

        private bool IsSorted()
        {
            var content = string.Join(Environment.NewLine, sortscript);
            const string command = "return statusSort.isSorted();";
            var js = string.Concat(content, Environment.NewLine, command);
            var response = JsExecutor.ExecuteScript(js);
            if (response is not bool isSorted) return false;
            return isSorted;
        }

        private void Sort()
        {
            var content = string.Join(Environment.NewLine, sortscript);
            const string command = "return statusSort.click();";
            var js = string.Concat(content, Environment.NewLine, command);
            JsExecutor.ExecuteScript(js);
        }

        private void Filter()
        {

            var content = string.Join(Environment.NewLine, filterscript);
            const string command = "// apply filter to open cases";
            var js = string.Concat(content, Environment.NewLine, command);
            JsExecutor.ExecuteScript(js);
        }

        private bool WaitForSelector()
        {
            try
            {

                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15)) { PollingInterval = TimeSpan.FromMilliseconds(400) };
                wait.Until(w =>
                {
                    var collection = w.TryFindElements(By.XPath(_sortLink));
                    if (collection == null) return false;
                    return collection.Any(x => x.Text == "Status");
                });
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to find selector");
                return false;
            }
        }

        private static readonly string[] filterscript = new[]
        {
            "try { ",
            "var casesgrid = $(\"#CasesGrid\").data(\"kendoGrid\");",
            "var ds = casesgrid.dataSource;",
            "ds.filter({ field: \"CaseStatusId.Description\", operator: \"eq\", value: \"OPEN\" }); } ",
            "catch { }"
        };

        private static readonly string[] sortscript = new[]
        {
            "var statusSort = { ",
            "	'getElement': function() { ",
            "		var arr = Array.prototype.slice.call( document.getElementsByTagName('a'), 0) ",
            "		.filter(x => { let attr = x.getAttribute('class'); return attr != null && attr == 'k-link'}) ",
            "		.filter(x => x.innerText == 'Status'); ",
            "		if (arr != null && arr.length > 0 ) return arr[0]; ",
            "		return null; ",
            "	}, ",
            "	isSorted: function(lnk) { ",
            "",
            "		if (undefined == lnk || null == lnk || !lnk ) { lnk = statusSort.getElement(); } ",
            "		var spns = lnk.getElementsByTagName('span'); ",
            "		if (null == spns || spns.length == 0) { return false; } ",
            "		var attr = spns[0].getAttribute('class'); ",
            "		if (attr == null || attr.indexOf('k-i-arrow-s') == -1) { return false; } ",
            "		return true; ",
            "	}, ",
            "	click: function() { ",
            "		var ele = statusSort.getElement(); ",
            "		if (ele == null) { return; } ",
            "		ele.click();  ",
            "	} ",
            "} "
        };
        private const string _sortLink = "//a[@class = 'k-link']";

        protected class NoCountHelper : BaseDallasSearchAction
        {
            public static bool IsNoCountData(IJavaScriptExecutor executor)
            {
                return IsNoCount(executor);
            }
        }

    }
}
