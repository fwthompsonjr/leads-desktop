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
            var result = DallasSearchProcess.GetRangeDtos(startDt, endDt);
            Assert.Equal(expected, result.Count);
        }

        [Theory]
        [InlineData("2024-09-01", "2024-09-30", 21)]
        [InlineData("2024-09-01", "2024-10-30", 43)]
        [InlineData("2024-09-01", "2024-11-15", 55)]
        public void GetBusinessDaysTest(string startDate, string endingDate, int expected)
        {
            var culture = CultureInfo.CurrentCulture;
            var startDt = DateTime.Parse(startDate, culture);
            var endDt = DateTime.Parse(endingDate, culture);
            var result = DallasSearchProcess.GetBusinessDays(startDt, endDt);
            Assert.Equal(expected, result.Count);
        }
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(12)]
        public void GetCourtNameById(int courtId)
        {
            var result = DallasSearchProcess.GetCourtName(courtId);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("nomatch")]
        [InlineData("justice")]
        [InlineData("county")]
        [InlineData("district")]
        [InlineData("District")]
        public void GetCourtNameFromText(string court)
        {
            var result = DallasSearchProcess.GetCourtName(court);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Theory]
        [InlineData("2024-09-01", "2024-09-07", "COUNTY")]
        [InlineData("2024-09-01", "2024-09-07", "DISTRICT")]
        [InlineData("2024-09-01", "2024-09-07", "JUSTICE")]
        public void ServiceCanGetInteractive(string startDate, string endingDate, string caseType)
        {
            var error = Record.Exception(() =>
            {
                var culture = CultureInfo.CurrentCulture;
                var startDt = DateTime.Parse(startDate, culture);
                var endDt = DateTime.Parse(endingDate, culture);
                var service = new DallasSearchProcess();
                service.SetSearchParameters(startDt, endDt, caseType);
                var web = service.GetUiInteractive();
                Assert.NotNull(web);
            });
            Assert.Null(error);

        }
    }
}
