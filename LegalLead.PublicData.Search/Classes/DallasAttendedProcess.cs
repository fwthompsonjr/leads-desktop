using System;
using System.Globalization;

namespace LegalLead.PublicData.Search.Classes
{
    public class DallasAttendedProcess
    {
        public string StartDate { get; private set; }
        public string EndingDate { get; private set; }
        public string CourtType { get; private set; }

        public void Search(DateTime startDate, DateTime endDate, string courtType)
        {
            const string fmt = "MM/dd/yyyy";
            CourtType = GetPrefix(startDate, courtType);
            StartDate = startDate.ToString(fmt, culture);
            EndingDate = endDate.ToString(fmt, culture);
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
                    return string.Concat("CC-", year, "*", month);
                case "DISTRICT":
                    return string.Concat("DC-", year, "*-", month);
                default:
                    return string.Concat("JPC-", year, "*", month);
            }
        }
        private static readonly CultureInfo culture = CultureInfo.InvariantCulture;
    }
}
