using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Classes
{
    public class TravisSearchProcess
    {
        public string StartDate { get; private set; }
        public string EndingDate { get; private set; }
        public List<string> CourtLocator { get; private set; }
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
        public TravisUiInteractive GetUiInteractive()
        {
            return new TravisUiInteractive(GetNavigationParameter());
        }

        public static List<DateRangeDto> GetRangeDtos(DateTime startDate, DateTime endingDate)
        {
            const string fmt = "yyyy"; // separate items by year
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
            if (CourtLocator == null || CourtLocator.Count == 0) return null;
            if (string.IsNullOrEmpty(CourtType)) return null;
            var courtLocator = string.Join("|", CourtLocator);
            var keys = new List<WebNavigationKey>
            {
                new() { Name = "StartDate", Value = StartDate  },
                new() { Name = "EndDate", Value = EndingDate  },
                new() { Name = "CourtLocator", Value = courtLocator  },
                new() { Name = "CourtType", Value = CourtType  }
            };

            return new WebNavigationParameter
            {
                Name = "TravisSearch",
                Keys = keys,
            };
        }


        private static List<string> GetPrefix(DateTime startDate, string courtType)
        {
            /*
                JUSTICE COURTS 
                Search J1-CV-YY* + DATE
                Search J2-CV-YY* + DATE
                Search J3-CV-YY* + DATE
                Search J4-CV-YY* + DATE
                Search J5-CV-YY* + DATE

                DISTRICT and COUNTY COURTS
                C-1-CV-YY*
                D-1-GN-YY*
            */
            var list = new List<string>();
            var year = startDate.ToString("yy", culture);
            switch (courtType)
            {
                case "COUNTY":
                    list.Add($"D-1-GN-{year}*");
                    break;
                case "DISTRICT":
                    list.Add($"C-1-CV-{year}*");
                    break;
                default:

                    for (int i = 1; i < 6; i++)
                    {
                        list.Add($"J{i}-CV-{year}*");
                    }
                    break;
            }
            return list;
        }
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private static readonly List<string> CourtNames = new() { "COUNTY", "DISTRICT", "JUSTICE" };
    }
}