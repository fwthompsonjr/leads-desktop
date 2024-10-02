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

        [Theory]
        [InlineData("2024-09-01", "2024-09-30", 21)]
        [InlineData("2024-09-01", "2024-10-30", 43)]
        [InlineData("2024-09-01", "2024-11-15", 55)]
        public void GetBusinessDaysTest(string startDate, string endingDate, int expected)
        {
            var culture = CultureInfo.CurrentCulture;
            var startDt = DateTime.Parse(startDate, culture);
            var endDt = DateTime.Parse(endingDate, culture);
            var result = DallasAttendedProcess.GetBusinessDays(startDt, endDt);
            Assert.Equal(expected, result.Count);
        }

        [Theory]
        [InlineData("2024-09-01", "2024-09-07", "COUNTY")]
        [InlineData("2024-09-01", "2024-09-07", "DISTRICT")]
        [InlineData("2024-09-01", "2024-09-07", "JUSTICE")]
        public void ServiceCanGetInteractive(string startDate, string endingDate, string caseType)
        {
            var error = Record.Exception(() => {
                var culture = CultureInfo.CurrentCulture;
                var startDt = DateTime.Parse(startDate, culture);
                var endDt = DateTime.Parse(endingDate, culture);
                var service = new DallasAttendedProcess();
                service.Search(startDt, endDt, caseType);
                var web = service.GetUiInteractive();
                Assert.NotNull(web);
            });
            Assert.Null(error);

        }
    }
}
