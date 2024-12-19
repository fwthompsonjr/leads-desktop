using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Classes
{
    public class DallasSearchProcess
    {
        public string StartDate { get; protected set; }
        public string EndingDate { get; protected set; }
        public string CourtLocator { get; protected set; }
        public string CourtType { get; protected set; }

        public void SetSearchParameters(DateTime? startDate, DateTime? endDate, string courtType)
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

        public static string GetCourtName(int courtId)
        {
            if (courtId < 0 || courtId > 2) return CourtNames[2];
            return CourtNames[courtId];
        }

        public static string GetCourtName(string courtName)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var id = CourtNames.FindIndex(x => x.Equals(courtName, oic));
            return GetCourtName(id);
        }
        public static List<DateTime> GetBusinessDays(DateTime startDate, DateTime endingDate,
            bool includeWeekend = false,
            bool includeHolidays = false)
        {
            var holidates = GetHolidayList();
            var list = new List<DateTime>();
            var begin = startDate.Date;
            var weekends =
                includeWeekend ? new() :
                new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            if (includeHolidays) holidates.RemoveAll(x => true);
            while (begin <= endingDate.Date)
            {
                if (!weekends.Contains(begin.DayOfWeek)) list.Add(begin);
                begin = begin.AddDays(1);
            }
            if (holidates.Count > 0 && list.Count > 0)
            {
                list.RemoveAll(x => holidates.Contains(x.Date));
            }
            return list.Distinct().ToList();
        }


        private static List<DateTime> GetHolidayList()
        {
            if (holidayQueries.Count > 0) return holidayQueries;
            var container = ActionSettingContainer.GetContainer;
            var instance = container.GetInstance<IRemoteDbHelper>();
            var response = instance.Holidays();
            if (response.Count == 0) return new();
            var temp = response
                .Select(x => x.HoliDate)
                .Where(x => x.HasValue)
                .Select(x => x.GetValueOrDefault().Date)
                .Distinct()
                .ToList();
            temp.Sort((a, b) => a.CompareTo(b));
            holidayQueries.AddRange(temp);
            return holidayQueries;
        }

        private WebNavigationParameter GetNavigationParameter()
        {
            if (string.IsNullOrEmpty(StartDate)) return null;
            if (string.IsNullOrEmpty(EndingDate)) return null;
            if (string.IsNullOrEmpty(CourtLocator) ||
                string.IsNullOrEmpty(CourtType)) return null;

            var keys = new List<WebNavigationKey>
            {
                new() { Name = "StartDate", Value = StartDate  },
                new() { Name = "EndDate", Value = EndingDate  },
                new() { Name = "CourtLocator", Value = CourtLocator  },
                new() { Name = "CourtType", Value = CourtType  }
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
            switch (courtType)
            {
                case "COUNTY":
                    return string.Concat("CC-", year, "-*");
                case "DISTRICT":
                    return string.Concat("DC-", year, "-*");
                default:
                    return string.Concat("JPC-", year, "*");
            }
        }
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private static readonly List<string> CourtNames = new() { "COUNTY", "DISTRICT", "JUSTICE" };
        private static readonly List<DateTime> holidayQueries = new();
    }
}
