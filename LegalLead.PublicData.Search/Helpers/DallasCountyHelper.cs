using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Enumerations;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegalLead.PublicData.Search.Helpers
{
    internal class DallasCountyHelper : DallasJusticeHelper
    {
        public DallasCountyHelper(
            IWebDriver web,
            IJavaScriptExecutor executor)
            : base(web, executor)
        {

            GetOfficers();
            Officers = JusticeOfficers;
            JsContentScript = JsSearchContent;
        }

        public override List<DallasJusticeOfficer> Officers { get; }
        public override string JsContentScript { get; protected set; }
        public override string Name => "COUNTY";

        public override ExecutionResponseType SetSearchParameter()
        {
            try
            {
                if (Driver == null || JsExecutor == null) return ExecutionResponseType.ValidationFail;
                if (JusticeOfficers.Count == 0) return ExecutionResponseType.None;
                if (SearchIndex < 0 || SearchIndex > JusticeOfficers.Count - 1) return ExecutionResponseType.IndexOfOutBounds;
                var officer = JusticeOfficers[SearchIndex];
                Console.WriteLine(" - Court location: {0}", officer.Court);
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
            if (JusticeOfficers.Count > 0) return JusticeOfficers;
            var list = new List<DallasJusticeOfficer>();
            if (Driver == null) return list;
            if (JsExecutor == null) return list;
            for (var i = 1; i < 6; i++)
            {
                var navTo = $"{NavAddress}{i}";
                if (!Uri.TryCreate(navTo, UriKind.Absolute, out var uri)) continue;
                var jo = new DallasJusticeOfficer { Court = $"CCL {i}" };
                Driver.Navigate().GoToUrl(uri);
                jo.Name = GetOfficerName();
                list.Add(jo);
            }
            JusticeOfficers.AddRange(list);
            return JusticeOfficers;
        }

        protected string GetOfficerName()
        {
            const char sq = (char)39;
            const char comma = ',';
            const char dot = '.';
            const char question = '?';
            var resp = JsExecutor.ExecuteScript(JsContent);
            if (resp is not string officer) return string.Empty;
            if (string.IsNullOrEmpty(officer)) return string.Empty;
            try
            {
                officer = officer.ToUpper();
                var cidx = officer.LastIndexOf(comma);
                if (cidx != -1) officer = officer[..cidx];
                cidx = officer.IndexOf(dot);
                if (cidx != -1) officer = officer[(cidx + 1)..].Trim();
                if (!officer.Contains(sq)) return officer;
                var builder = new StringBuilder(officer);
                builder.Replace(sq, question);
                return builder.ToString();
            }
            catch (Exception)
            {
                return officer;
            }
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

        private static string jscontent = null;
        private static readonly string[] blockjs = new string[]
        {
            "var spanish = { ",
            "	'table': ",
            "	 [ ",
            "		{ 'letter': 'A', 'code': String.fromCharCode(193) }, ",
            "		{ 'letter': 'a', 'code': String.fromCharCode(225) }, ",
            "		{ 'letter': 'E', 'code': String.fromCharCode(201) }, ",
            "		{ 'letter': 'e', 'code': String.fromCharCode(233) }, ",
            "		{ 'letter': 'I', 'code': String.fromCharCode(205) }, ",
            "		{ 'letter': 'i', 'code': String.fromCharCode(237) }, ",
            "		{ 'letter': 'O', 'code': String.fromCharCode(211) }, ",
            "		{ 'letter': 'o', 'code': String.fromCharCode(243) }, ",
            "		{ 'letter': 'U', 'code': String.fromCharCode(218) }, ",
            "		{ 'letter': 'u', 'code': String.fromCharCode(250) }, ",
            "		{ 'letter': 'N', 'code': String.fromCharCode(209) }, ",
            "		{ 'letter': 'n', 'code': String.fromCharCode(241) } ",
            "	], ",
            "	'sanitize': function( original ) { ",
            "		if (null == original || original.length == 0 ) { return ''; } ",
            "		spanish.table.forEach(t => { ",
            "			while ( original.indexOf(t.code) >= 0 ) { ",
            "				original = original.replace(t.code, t.letter); ",
            "			} ",
            "		}); ",
            "		return original; ",
            "	} ",
            "} ",
            "var getJudge = { ",
            "	'find': function() { ",
            "		var headers = Array.prototype.slice.call( ",
            "		  document.getElementsByTagName('h2'), 0); ",
            "		var text = headers[0].innerText; ",
            "		text = spanish.sanitize(text); ",
            "		return text; ",
            "	} ",
            "} ",
            " ",
            "return getJudge.find(); "
        };
        private static readonly List<DallasJusticeOfficer> JusticeOfficers = new();
        private const string NavAddress = "https://www.dallascounty.org/government/courts/county_court_at_law/law";
    }
}