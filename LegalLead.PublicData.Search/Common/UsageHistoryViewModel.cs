using Newtonsoft.Json;
using System;
using System.Globalization;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Common
{
    public class UsageHistoryViewModel
    {
        public string CountyName { get; set; }
        public string SearchDates { get; set; }

        public int RecordCount { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }

        internal static UsageHistoryViewModel ConvertFrom(UsageHistoryModel model)
        {
            var incidentDate = new DateTime(model.CreateDate.Ticks, DateTimeKind.Utc).ToLocalTime();
            return new()
            {
                CountyName = textConverter.ToTitleCase(model.CountyName),
                RecordCount = model.MonthlyUsage,
                SearchDates = model.DateRange,
                CreateDate = incidentDate,
            };
        }
        private static readonly TextInfo textConverter = new CultureInfo("en-US", false).TextInfo;
    }
}
