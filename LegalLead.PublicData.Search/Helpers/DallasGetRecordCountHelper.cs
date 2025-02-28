using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Extensions;

namespace LegalLead.PublicData.Search.Helpers
{
    public class DallasGetRecordCountHelper : DallasSortByStatusHelper
    {
        private readonly string SetMaxRowsScript;
        public DallasGetRecordCountHelper(
            IWebDriver driver,
            IJavaScriptExecutor executor,
            string setPagerScript) : base(driver, executor)
        {
            SetMaxRowsScript = setPagerScript;
            statusResponse.Actual = 0;
            statusResponse.Expected = 0;
        }

        public override void Execute()
        {
            const int retryCount = 300;
            const int waitMilliSeconds = 500;
            if (NoCountHelper.IsNoCountData(JsExecutor)) return;
            if (!WaitForSelector()) return;
            var retries = retryCount;
            while (retries > 0)
            {
                if (retries == retryCount || retries % 10 == 0)
                {
                    JsExecutor.ExecuteScript(SetMaxRowsScript);
                    Thread.Sleep(waitMilliSeconds * 2);
                }
                GetRecordCount();
                if (IsTableDataLoaded()) break;
                retries--;
            }
        }

        private bool IsTableDataLoaded()
        {
            return statusResponse.IsMatched();
        }

        private void GetRecordCount()
        {
            var content = string.Join(Environment.NewLine, jsGetRecordCount);
            var rsp = JsExecutor.ExecuteScript(content);
            if (rsp is not string json) return;
            var obj = json.ToInstance<RecordStatusResponse>();
            if (obj == null) return;
            lock (locker)
            {
                statusResponse.Expected = obj.Expected;
                statusResponse.Actual = obj.Actual;
            }
        }

        private bool WaitForSelector()
        {
            try
            {

                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500)
                };
                wait.Until(w =>
                {
                    var collection = w.TryFindElement(By.XPath(_tableDef));
                    return collection != null;
                });
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to find selector");
                return false;
            }
        }

        private static readonly string[] jsGetRecordCount = new[]
        {
            "var rc_counter = { ",
            "    'get_row_count_label' : function() { ",
            "    const findClassName = 'k-pager-info k-label' ",
            "    var list = document.getElementsByTagName('span'); ",
            "    var arr = Array.prototype.slice.call( list, 0 ); ",
            "    var find = arr.filter(a => {  ",
            "        let attr = a.getAttribute('class');  ",
            "        if (attr == null) { return false; }  ",
            "        return attr == findClassName }); ",
            "    if (find == null || find.length == 0) { ",
            "        return null ",
            "    }  ",
            "    return find[0]; ",
            "    }, ",
            "    'get_records_table' : function() { ",
            "    const findClassName = 'kgrid-card-table' ",
            "    var list = document.getElementsByTagName('table'); ",
            "    var arr = Array.prototype.slice.call( list, 0 ); ",
            "    var find = arr.filter(a => {  ",
            "        let attr = a.getAttribute('class');  ",
            "        if (attr == null) { return false; }  ",
            "        return attr == findClassName }); ",
            "    if (find == null || find.length == 0) { ",
            "        return null ",
            "    }  ",
            "    return find[0]; ",
            "    }, ",
            "    'get_record_elements' : function() { ",
            "    let tbl = rc_counter.get_records_table() ",
            "    if ( null == tbl ) { return null; } ",
            "    let rws = tbl.getElementsByTagName('a'); ",
            "    let arrows = Array.prototype.slice.call( rws, 0 ); ",
            "    arrows = arrows.filter(a => { ",
            "        let attr = a.getAttribute('class'); ",
            "        if (attr == null) { return false; }  ",
            "        return attr == 'caseLink' }); ",
            "    return arrows; ",
            "    }, ",
            "    'get_record_count' : function() { ",
            "    let items = rc_counter.get_record_elements() ",
            "    if ( null == items ) { return 0; } ",
            "    return items.length; ",
            "    }, ",
            "    'get_expected_count' : function() { ",
            "        const spc = ' ' ",
            "        let spn = rc_counter.get_row_count_label(); ",
            "        if (spn == null) { return 0; } ",
            "        let txt = spn.innerText ",
            "        if (txt.length == 0 || txt.indexOf(spc) < 0 ) { return 0; } ",
            "        let arr = txt.split(spc) ",
            "        let ln = arr.length ",
            "        if (ln < 6) { return 0; } ",
            "        let itm = arr[ln-2] ",
            "        let nbr = parseInt(itm) ",
            "        if (isNaN(nbr)) { return 0 } ",
            "        return nbr ",
            "    }, ",
            "    'get_status' : function() { ",
            "        let actual = rc_counter.get_record_count() ",
            "        let expected = rc_counter.get_expected_count(); ",
            "        let obj = { ",
            "            'actual': actual, ",
            "            'expected': expected ",
            "        } ",
            "        return JSON.stringify(obj) ",
            "    } ",
            "} ",
            " ",
            "return rc_counter.get_status() ",
        };
        private readonly RecordStatusResponse statusResponse = new();

        private const string _tableDef = "//a[@class = 'caseLink'][@data-caseid]";
        private sealed class RecordStatusResponse
        {
            public int Expected { get; set; }
            public int Actual { get; set; }
            public bool IsMatched()
            {
                return Actual.Equals(Expected) && Actual > 0
                    || Actual > Expected
                    || Actual.Equals(Expected) && Expected == 0;
            }
        }

        private static readonly object locker = new();
    }
}