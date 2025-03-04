using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Enumerations;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LegalLead.PublicData.Search.Helpers
{
    internal class DallasDistrictHelper : DallasCountyHelper
    {
        public DallasDistrictHelper(
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
        public override string Name => "DISTRICT";
        public bool IsDistrictFilterActive { get; set; }
        public override ExecutionResponseType SetSearchParameter()
        {
            try
            {
                if (Driver == null || JsExecutor == null) return ExecutionResponseType.ValidationFail;
                if (JusticeOfficers.Count == 0) return ExecutionResponseType.None;
                if (SearchIndex < 0 || SearchIndex > JusticeOfficers.Count - 1) return ExecutionResponseType.IndexOfOutBounds;
                var officer = JusticeOfficers[SearchIndex];
                if (!IsDistrictFilterActive) { return ExecutionResponseType.Success; }
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
            districtCourtNumbers.ForEach(i =>
            {
                var navTo = $"{NavAddress}{GetOrdinal(i)}/";
                if (Uri.TryCreate(navTo, UriKind.Absolute, out var uri))
                {
                    var jo = new DallasJusticeOfficer { Court = $"DC {i}" };
                    Driver.Navigate().GoToUrl(uri);
                    jo.Name = GetOfficerName();
                    list.Add(jo);
                }
            });
            JusticeOfficers.AddRange(list);
            return JusticeOfficers;
        }


        private readonly List<string> officerNames = new();
        private static string GetOrdinal(int number)
        {
            var formatter = CultureInfo.CurrentCulture.NumberFormat;
            var nbr = number.ToString(formatter);
            var lastChar = Convert.ToInt32(nbr[^1].ToString(), formatter);
            var ordinal = "th";
            if (lastChar == 1) ordinal = "st";
            if (lastChar == 2) ordinal = "nd";
            if (lastChar == 3) ordinal = "rd";
            return $"{nbr}{ordinal}";
        }
        private static readonly List<int> districtCourtNumbers = new()
        {
            14,
            44,
            68,
            95,
            101,
            134,
            160,
            162,
            191,
            193,
            298
        };
        private static readonly List<DallasJusticeOfficer> JusticeOfficers = new();
        private const string NavAddress = "https://www.dallascounty.org/government/courts/civil_district/";
    }
}
