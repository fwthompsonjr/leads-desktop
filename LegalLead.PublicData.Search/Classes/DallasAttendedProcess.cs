using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.PublicData.Search.Classes
{
    internal class DallasAttendedProcess
    {
        public string StartDate { get; private set; }
        public string EndingDate { get; private set; }
        public string CourtType { get; private set; }

        public void Search(DateTime startDate, DateTime endDate, string courtType)
        {
            const string fmt = "MM/dd/yyyy";
            CourtType = GetPrefix(startDate, courtType);
            StartDate = startDate.ToString(fmt);
            EndingDate = endDate.ToString(fmt);
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
            switch (courtType)
            {
                case "COUNTY":
                    return string.Concat("CC-", startDate.ToString("yy"), "*", startDate.Month.ToString("MM"));
                case "DISTRICT":
                    return string.Concat("DC-", startDate.ToString("yy"), "*-", startDate.Month.ToString("MM"));
                default:
                    return string.Concat("JPC-", startDate.ToString("yy"), "*", startDate.Month.ToString("MM"));
            }
        }

    }
}
