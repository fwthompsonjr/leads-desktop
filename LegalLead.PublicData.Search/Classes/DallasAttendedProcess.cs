using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Classes
{
    public class DallasAttendedProcess
    {
        public string StartDate { get; private set; }
        public string EndingDate { get; private set; }
        public string CourtLocator { get; private set; }
        public string CourtType { get; private set; }

        public void Search(DateTime? startDate, DateTime? endDate, string courtType)
        {
            const string fmt = "MM/dd/yyyy";
            var name = CourtNames.Find(x => x.Equals(courtType, StringComparison.OrdinalIgnoreCase));
            if (startDate != null)
            {
                StartDate = startDate.Value.ToString(fmt, culture);
            }
            if (!string.IsNullOrEmpty(name))
            {
                CourtType = name;
                CourtLocator = GetPrefix(startDate.GetValueOrDefault(), name);
            }
            if (endDate != null)
            {
                EndingDate = endDate.Value.ToString(fmt, culture);
            }
        }
        public DallasUiInteractive GetUiInteractive()
        {
            return new DallasUiInteractive(GetNavigationParameter());
        }

        public static List<DateRangeDto> GetRangeDtos(DateTime startDate, DateTime endingDate)
        {
            const string fmt = "yyyy-MM";
            var businessDays = GetBusinessDays(startDate, endingDate);
            var groupa = startDate.ToString(fmt, culture);
            var groups = businessDays.Select(x => new { indx = x.ToString(fmt, culture), date = x });
            var collection = new List<DateRangeDto>();
            var one = groups.Where(x => x.indx == groupa).Select(x => x.date).ToArray();
            var two = groups.Where(x => x.indx != groupa).Select(x => x.date).ToArray();
            collection.Add(new DateRangeDto
            {
                StartDate = one.Min(),
                EndDate = one.Max()
            });
            if (two.Length == 0) return collection;
            collection.Add(new DateRangeDto
            {
                StartDate = two.Min(),
                EndDate = two.Max()
            });
            return collection;
        }


        public static List<DateTime> GetBusinessDays(DateTime startDate, DateTime endingDate)
        {
            var list = new List<DateTime>();
            var begin = startDate.Date;
            var weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            while (begin <= endingDate.Date)
            {
                if (!weekends.Contains(begin.DayOfWeek)) list.Add(begin);
                begin = begin.AddDays(1);
            }
            return list.Distinct().ToList();
        }

        private WebNavigationParameter GetNavigationParameter()
        {
            if (string.IsNullOrEmpty(StartDate)) return null;
            if (string.IsNullOrEmpty(EndingDate)) return null;
            if (string.IsNullOrEmpty(CourtLocator) ||
                string.IsNullOrEmpty(CourtType)) return null;

            var keys = new List<WebNavigationKey>
            {
                new WebNavigationKey { Name = "StartDate", Value = StartDate  },
                new WebNavigationKey { Name = "EndDate", Value = EndingDate  },
                new WebNavigationKey { Name = "CourtLocator", Value = CourtLocator  },
                new WebNavigationKey { Name = "CourtType", Value = CourtType  }
            };

            return new WebNavigationParameter
            {
                Name = "DallasSearch",
                Keys = keys,
            };
        }


        private static string GetPrefix(DateTime startDate, string courtType)
        {
            /*
                JUSTICE COURTS 
                Search: JPC-YY* + DATE
                DISTRICT COURTS
                Search:  DC-YY-MM* + DATE
                COUNTY COURTS
                Search: CC-YY* + DATE
            */
            var year = startDate.ToString("yy", culture);
            var month = startDate.ToString("MM", culture);
            switch (courtType)
            {
                case "COUNTY":
                    return string.Concat("CC-", year, "*", month, "*");
                case "DISTRICT":
                    return string.Concat("DC-", year, "*-", month, "*");
                default:
                    return string.Concat("JPC-", year, "*", month, "*");
            }
        }
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private static readonly List<string> CourtNames = new List<string> { "COUNTY", "DISTRICT", "JUSTICE" };
    }
}
