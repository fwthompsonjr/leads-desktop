using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Enumerations;
using LegalLead.PublicData.Search.Interfaces;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Helpers
{
    internal class DallasJusticeHelper : BaseCaseTypeHelper
    {
        public DallasJusticeHelper(
            IWebDriver web,
            IJavaScriptExecutor executor) 
            : base(web, executor)
        {
            
        }

        public override string Name => "JUSTICE";
        public override int SearchIndex { get; set; }
        public override ExecutionResponseType SetSearchParameter()
        {
            try
            {
                if (Driver == null || JsExecutor == null) return ExecutionResponseType.ValidationFail;
                if (JusticeOfficers.Count == 0) return ExecutionResponseType.None;
                if (SearchIndex < 0 || SearchIndex > JusticeOfficers.Count - 1) return ExecutionResponseType.IndexOfOutBounds;
                var officer = JusticeOfficers[SearchIndex];
                var js = JsSearchContent.Replace("~0", officer.Name);
                var actual = JsExecutor.ExecuteScript(js);
                if (actual is not bool response) return ExecutionResponseType.ExecutionFailed;
                return response ? ExecutionResponseType.Success : ExecutionResponseType.ExecutionFailed;
            }
            catch
            {
                return ExecutionResponseType.UnexpectedError;
            }
        }

        protected override List<string> ParameterList
        {
            get
            {
                if (GetOfficers().Count == 0) return new();
                if (officerNames.Count > 0) return officerNames;
                var names = GetOfficers().Select(x => x.Name);
                officerNames.AddRange(names);
                return officerNames;
            }
        }
        private List<DallasJusticeOfficer> GetOfficers()
        {
            const char sq = (char)39;
            const char comma = ',';
            const char question = '?';
            if (JusticeOfficers.Count > 0) return JusticeOfficers;
            var list = new List<DallasJusticeOfficer>();
            if (Driver == null) return list;
            if (JsExecutor == null) return list;
            if (!Uri.TryCreate(NavAddress, UriKind.Absolute, out var uri)) return list;
            Driver.Navigate().GoToUrl(uri);
            var content = JsExecutor.ExecuteScript(JsContent);
            if (content is not string js) return list;
            var arr = JsonConvert.DeserializeObject<List<string>>(js);
            if (arr == null || arr.Count == 0) return list;
            arr.ForEach(a =>
            {
                if (a.Contains(comma))
                {
                    var items = a.Split(comma);
                    var bldr = new StringBuilder(items[0].Trim().ToUpper());
                    bldr.Replace(sq, question);
                    var nme = bldr.ToString();
                    list.Add(new()
                    {
                        Name = nme,
                        Court = items[^1].Trim()
                    });
                }
            });
            JusticeOfficers.AddRange(list);
            return JusticeOfficers;
        }
        private readonly List<string> officerNames = new();
        private static string JsContent
        {
            get
            {
                if (!string.IsNullOrEmpty(jscontent)) return jscontent;
                jscontent = string.Join(Environment.NewLine, blockjs);
                return jscontent;
            }
        }
        private static string JsSearchContent
        {
            get
            {
                if (!string.IsNullOrEmpty(searchjscontent)) return searchjscontent;
                searchjscontent = string.Join(Environment.NewLine, searchjs);
                return searchjscontent;
            }
        }

        private static string jscontent = null;
        private static string searchjscontent = null;
        private static readonly string[] blockjs = new string[]
        {
            "var js_justice = { ",
            "'get_officers': function() ",
            "  { ",
            "	var ofclist = []; ",
            "	try { ",
            "	var dvfind = 'dc-sidebar-collapsible dc_accordian'; ",
            "	var linkfind = 'government/jpcourts'; ",
            "	var dvaccordian = Array.prototype.slice.call( ",
            "	   document.getElementsByTagName('div'), 0) ",
            "	  .find(a => { ",
            "		let cls = a.getAttribute('class'); ",
            "		return cls != null && cls == dvfind; ",
            "	}); ",
            "	var aa = Array.prototype.slice.call( dvaccordian.getElementsByTagName('a'), 0); ",
            "	aa = aa.filter(a => { return a.innerText.indexOf(',') > 0}); ",
            "	bb = aa.map(x => x.innerText); ",
            "	return JSON.stringify(bb); ",
            "	} catch { ",
            "	return JSON.stringify(ofclist); ",
            "	} ",
            "  } ",
            "}; ",
            "return js_justice.get_officers(); "
        };
        private static readonly string[] searchjs = new string[]
        {
            "var js_jdo = ",
            "{ ",
            "	'db': [], ",
            "	'jq_check': function() { ",
            "		var hasJq = (window.jQuery != null);  ",
            "		if (hasJq) { return; } ",
            "		var jq = document.createElement('script'); ",
            "		jq.src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js'; ",
            "		document.getElementsByTagName('head')[0].appendChild(jq); ",
            "	}, ",
            "	'init': function() { ",
            "		js_jdo.jq_check(); ",
            "		if (js_jdo.db.length > 0) { return; } ",
            "		var dsrc = '\"dataSource\":'; ",
            "		var jocontainter = document.getElementById('SearchCaseJudicialOfficerContainer'); ",
            "		var jqtext = jocontainter.getElementsByTagName('script')[0].innerHTML; ",
            "		var kendoid = jqtext.indexOf('.kendoComboBox(') ",
            "		jqtext = jqtext.substr(kendoid); ",
            "		bracketid = jqtext.indexOf(dsrc) ",
            "		jqtext = jqtext.substr(bracketid); ",
            "		bracketid = jqtext.indexOf(']'); ",
            "		jqtext = jqtext.substr(0, bracketid + 1).replace(dsrc, ''); ",
            "		var data = JSON.parse(jqtext); ",
            "		data.forEach(d => js_jdo.db.push(d)); ",
            "		 ",
            "		const cmbo = 'caseCriteria_JudicialOfficer'; ",
            "		let jcmbo = ''.concat('#', cmbo); ",
            "		$(jcmbo).init(); ",
            "		return; ",
            "	}, ",
            "	'find': function(name) { ",
            "		js_jdo.init(); ",
            "		let qs = `?`; ",
            "		let bq = `'`; ",
            "		while (name.indexOf(qs) >= 0) { name = name.replace( qs, bq); } ",
            "		var items = name.split(' ').map(x => x.toUpperCase()); ",
            "		var subset = []; ",
            "		indx = items.length - 1; ",
            "		js_jdo.db.forEach(x =>  ",
            "			{  ",
            "				let isFound = String(x['Text']).toUpperCase().indexOf(items[0]) >= 0; ",
            "				isFound &= String(x['Text']).toUpperCase().indexOf(items[indx]) >= 0; ",
            "				if (isFound) subset.push(x); ",
            "			}); ",
            "		if (subset.length <= 1) { return subset; } ",
            "		indx--; ",
            "		subset = subset.filter(x =>  ",
            "			{  ",
            "				let isFound = String(x['Text']).toUpperCase().indexOf(items[indx]) >= 0; ",
            "				if (isFound) subset.push(x); ",
            "			}); ",
            "		return subset; ",
            "	}, ",
            "	'set_combo': function(name) { ",
            "		try { ",
            "			const kndbx = 'kendoComboBox'; ",
            "			const cmbo = 'caseCriteria_JudicialOfficer'; ",
            "			let jcmbo = ''.concat('#', cmbo); ",
            "			let arr = js_jdo.find(name); ",
            "			if (arr.length != 1) { return false; } ",
            "			let vlu = arr[0]['Value']; ",
            "			if (vlu == null) return false; ",
            "			console.log( vlu ); ",
            "			let cbx = document.getElementById(cmbo); ",
            "			if (undefined == $(jcmbo).data(kndbx)) { return false; } ",
            "			$(jcmbo).data(kndbx).value(vlu); ",
            "			cbx.dispatchEvent(new Event('change')); ",
            "			return true; ",
            "		} catch { ",
            "			return false; ",
            "		} ",
            "	} ",
            "} ",
            "",
            "return js_jdo.set_combo( '~0' );",
        };
        private static readonly List<DallasJusticeOfficer> JusticeOfficers = new();
        private const string NavAddress = "https://www.dallascounty.org/government/jpcourts/";
    }
}
