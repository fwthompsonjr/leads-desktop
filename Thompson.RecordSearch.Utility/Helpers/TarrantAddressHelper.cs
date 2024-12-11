using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Helpers
{
    internal class TarrantAddressHelper
    {
        private readonly IWebDriver driver;
        private readonly IJavaScriptExecutor jsExec;
        private readonly TarrantWebInteractive twInteractive;
        private readonly HLinkDataRow dataRow;
        public TarrantAddressHelper(
            IWebDriver theDriver,
            TarrantWebInteractive interactive,
            HLinkDataRow linkData)
        {
            driver = theDriver;
            twInteractive = interactive;
            dataRow = linkData;
            jsExec = (IJavaScriptExecutor)theDriver;
        }

        public bool GetAddressInformation()
        {
            var fmt = twInteractive.GetParameterValue<string>(CommonKeyIndexes.HlinkUri);
            var condition = By.XPath("/html/body/table[3]/tbody/tr/td[1]/b");
            var helper = new ElementAssertion(driver);
            helper.Navigate(string.Format(CultureInfo.CurrentCulture, fmt, dataRow.WebAddress));
            var tdName = WaitForElement(condition);
            if (tdName == null) return false;
            var body = jsExec.ExecuteScript(JsScript);
            if (!(body is string jsbody)) return false;
            var dataPoint = GetDataPointLocator();
            if (dataPoint == null) return false;
            var jsResponse = JsonConvert.DeserializeObject<JsResponse>(jsbody);
            if (jsResponse == null) return false;
            var address = jsResponse.Address.Split("\n");
            dataPoint.Result = jsResponse.CaseStyle;
            dataRow.PageHtml = JsonConvert.SerializeObject(dataPoint);
            dataRow.Defendant = jsResponse.DefendantName;
            dataRow.Address = string.Join("<br/>", address);
            dataRow.IsCriminal = jsResponse.IsCriminal;
            return true;
        }
        private IWebElement WaitForElement(By condition)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500)
                };
                wait.Until(d =>
                {
                    return d.TryFindElement(condition) != null;
                });
                return driver.TryFindElement(condition);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static DataPoint GetDataPointLocator()
        {
            var dto = DataPointLocatorDto.GetDto("tarrantCountyDataPoint");
            var search = dto.DataPoints.FirstOrDefault(x =>
                x.Name.Equals(CommonKeyIndexes.CaseStyle, System.StringComparison.CurrentCultureIgnoreCase));
            if (search == null) return null;
            var tmp = JsonConvert.SerializeObject(search);
            var obj = JsonConvert.DeserializeObject<DataPoint>(tmp) ?? new DataPoint();
            return obj;

        }
        private static string JsScript
        {
            get
            {
                if (!string.IsNullOrEmpty(jsScript)) return jsScript;
                jsScript = string.Join(Environment.NewLine, jsFinder);
                return jsScript;
            }
        }
        private static string jsScript = null;
        private static readonly List<string> jsFinder = new List<string>()
        {
            "var js_finder = { ",
            "	'isCriminal' : function() { ",
            "		var links = Array.prototype.slice.call( document.getElementsByTagName('a'), 0 ); ",
            "		links = links.filter(a => {  ",
            "		  let attr = a.getAttribute('class');  ",
            "		  if (attr == null) { return false; }  ",
            "		  if (attr != 'ssBlackNavBarHyperlink') return false;  ",
            "		  return a.innerText.indexOf('Criminal') >= 0 }); ",
            "		  return links.length > 0; ",
            "	}, ",
            "	'getAddress': function(table, rwid) { ",
            "		const nbs = '&nbsp;';",
            "		var trows = Array.prototype.slice.call( table.getElementsByTagName('tr'), 0 ); ",
            "		if (trows.length < rwid ) return ''; ",
            "		var tmp = trows[rwid].innerHTML;",
            "		while (tmp.indexOf(nbs) >= 0) { tmp = tmp.replace(nbs, ' '); }",
            "		trows[rwid].innerHTML = tmp;",
            "		return trows[rwid].innerText.trim(); ",
            "	}, ",
            "	'getStyle': function() { ",
            "		try ",
            "		{ ",
            "			var tblist = Array.prototype.slice.call( document.getElementsByTagName('table'), 0 ); ",
            "			tblist = tblist.filter(x => { ",
            "				let txt = x.innerHTML; ",
            "				if (txt.indexOf('Case Type:') < 0) { return false; } ",
            "				if (txt.indexOf('Date Filed:') < 0) { return false; } ",
            "				if (txt.indexOf('ssTableHeaderLabel') < 0) { return false; } ",
            "				return true; ",
            "			}); ",
            " ",
            "			var cstlye = tblist[0] ",
            "			.getElementsByTagName('tbody')[0] ",
            "			.getElementsByTagName('tr')[0] ",
            "			.getElementsByTagName('td')[0] ",
            "			.getElementsByTagName('b')[0]; ",
            "			return cstlye.innerText.trim(); ",
            "		} ",
            "		catch  ",
            "		{ ",
            "			return ''; ",
            "		} ",
            "	}, ",
            "	'getDefendantJson': function() { ",
            "		var rsp = { ",
            "			'rowid': -1, ",
            "			'defendant': '', ",
            "			'address': '', ",
            "			'caseStyle': '', ",
            "			'isCriminal': false, ",
            "			'table': '' ",
            "		}; ",
            "		try ",
            "		{ ",
            "			rsp.isCriminal = js_finder.isCriminal(); ",
            "			rsp.caseStyle = js_finder.getStyle(); ",
            "			var td = document.getElementById('PIr11'); ",
            "			if (null == td) { return JSON.stringify(rsp) } ",
            "			rsp.defendant = td.innerText.trim(); ",
            "			var row = td.closest('tr'); ",
            "			if (null == row) { return JSON.stringify(rsp) } ",
            "			var rowid = row.rowIndex; ",
            "			rsp.rowid = rowid; ",
            "			rowid = rowid + 1; ",
            "			var table = td.closest('table'); ",
            "			if (null == table) { return JSON.stringify(rsp) } ",
            "			rsp.address = js_finder.getAddress(table, rowid); ",
            "			rsp.table = table.outerHTML; ",
            "			return JSON.stringify(rsp); ",
            "		} ",
            "		catch ",
            "		{ ",
            "			return JSON.stringify(rsp); ",
            "		} ",
            "	} ",
            "}; return js_finder.getDefendantJson(); "
        };
        private sealed class JsResponse
        {
            [JsonProperty("rowid")] public int RowId { get; set; }
            [JsonProperty("defendant")] public string DefendantName { get; set; }
            [JsonProperty("address")] public string Address { get; set; }
            [JsonProperty("caseStyle")] public string CaseStyle { get; set; }
            [JsonProperty("isCriminal")] public bool IsCriminal { get; set; }
            [JsonProperty("table")] public string Table { get; set; }
        }
    }
}
