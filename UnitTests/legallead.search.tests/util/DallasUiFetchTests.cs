using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Util;
using Moq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Models;

namespace legallead.search.tests.util
{
    public class DallasUiFetchTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServiceRequiresParameters(bool show)
        {
            Assert.Throws<ArgumentNullException>(() => { _ = new DallasUiInteractive(null, show); });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ServiceRequiresValidParameters(int conditionId)
        {
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = "2024-05-25"},
                new() { Name = "EndDate", Value = "2024-05-25"},
                new() { Name = "CourtType", Value = "JUSTICE"}
            };
            if (conditionId == 0) keys[0].Value = "not-a-date";
            if (conditionId == 1) keys[1].Value = "not-a-date";
            if (conditionId == 2) keys.RemoveAt(0);
            if (conditionId == 3) keys.RemoveAt(1);
            if (conditionId == 4) keys.RemoveAt(2);
            var wb = new WebNavigationParameter { Keys = keys };
            Assert.Throws<ArgumentOutOfRangeException>(() => { _ = new DallasUiInteractive(wb); });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServiceCanGetDriver(bool show)
        {
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = "2024-05-25"},
                new() { Name = "EndDate", Value = "2024-05-25"},
                new() { Name = "CourtType", Value = "JUSTICE"} };
            var wb = new WebNavigationParameter { Keys = keys };
            var service = new DallasUiInteractive(wb);
            using var driver = service.GetDriver(show);
            Assert.NotNull(driver);
        }


        [Fact]
        public void ServiceCanFetch()
        {
            var error = Record.Exception(() =>
            {
                var keys = new List<WebNavigationKey> {
                    new() { Name = "StartDate", Value = "2024-05-25"},
                    new() { Name = "EndDate", Value = "2024-05-25"},
                    new() { Name = "CourtType", Value = "JUSTICE"}
                };
                var wb = new WebNavigationParameter { Keys = keys };
                var service = new NoIteratingWeb(wb);
                _ = service.Fetch();
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void ServiceCanIterate(int conditionId)
        {
            var error = Record.Exception(() =>
            {
                var keys = new List<WebNavigationKey> {
                    new() { Name = "StartDate", Value = "2024-05-25"},
                    new() { Name = "EndDate", Value = "2024-05-25"},
                    new() { Name = "CourtType", Value = "JUSTICE"}
                };
                var wb = new WebNavigationParameter { Keys = keys };
                var service = new IteratingWeb(wb);
                service.CheckIteration(conditionId);
            });
            Assert.Null(error);
        }

        private sealed class NoIteratingWeb : DallasUiInteractive
        {
            public NoIteratingWeb(WebNavigationParameter parameters, bool displayDialogue = true) : base(parameters, displayDialogue)
            {
            }
            public override IWebDriver GetDriver(bool headless = false)
            {
                var mock = new Mock<IWebDriver>();
                return mock.Object;
            }
            protected override void Iterate(IWebDriver driver, DallasAttendedProcess parameters, List<DateTime> dates, List<IDallasAction> common, List<IDallasAction> postcommon)
            {
            }
        }

        private sealed class IteratingWeb : DallasUiInteractive
        {
            public IteratingWeb(WebNavigationParameter parameters, bool displayDialogue = true) : base(parameters, displayDialogue)
            {
            }
            public override IWebDriver GetDriver(bool headless = false)
            {
                var mock = new Mock<IWebDriver>();
                return mock.Object;
            }

            public void CheckIteration(int conditionId)
            {
                var driver = GetDriver();
                var prc = new DallasAttendedProcess();
                List<DateTime> dates = [];
                List<IDallasAction> common = [];
                List<IDallasAction> postcommon = [];
                if (conditionId == 0) Iterate(null, prc, dates, common, postcommon);
                if (conditionId == 1) Iterate(driver, null, dates, common, postcommon);
                if (conditionId == 2) Iterate(driver, prc, null, common, postcommon);
                if (conditionId == 3) Iterate(driver, prc, dates, null, postcommon);
                if (conditionId == 4) Iterate(driver, prc, dates, common, null);
                if (conditionId == 5) Iterate(driver, prc, dates, common, postcommon);
            }

            protected override void IterateDateRange(IWebDriver driver, DallasAttendedProcess parameters, List<DateTime> dates, List<IDallasAction> common)
            {
            }

            protected override void IterateItems(IWebDriver driver, DallasAttendedProcess parameters, List<IDallasAction> postcommon)
            {
            }
        }
    }
}
