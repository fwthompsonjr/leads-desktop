using LegalLead.PublicData.Search.Classes;
using System;
using System.Globalization;

namespace legallead.search.tests.classes
{
    public class DallasAttendedProcessTests
    {
        [Theory]
        [InlineData("2024-09-01", "2024-09-30", 1)]
        [InlineData("2024-09-01", "2024-10-30", 2)]
        [InlineData("2024-09-01", "2024-11-15", 2)]
        public void GetRangeDtos(string startDate, string endingDate, int expected)
        {
            var culture = CultureInfo.CurrentCulture;
            var startDt = DateTime.Parse(startDate, culture);
            var endDt = DateTime.Parse(endingDate, culture);
            var result = DallasAttendedProcess.GetRangeDtos(startDt, endDt);
            Assert.Equal(expected, result.Count);
        }
    }
}
