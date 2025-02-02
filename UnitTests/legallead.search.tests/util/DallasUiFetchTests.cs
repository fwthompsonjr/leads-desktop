using Bogus;
using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using Moq;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
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
            if (!Debugger.IsAttached) return;
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = "2024-05-25"},
                new() { Name = "EndDate", Value = "2024-05-25"},
                new() { Name = "CourtType", Value = "JUSTICE"} };
            var wb = new WebNavigationParameter { Keys = keys };
            var service = new DallasUiInteractive(wb);
            using var driver = service.GetDriver(show);
            driver.Close();
            Assert.NotNull(driver);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ServiceCanFetch(int instanceId)
        {
            var error = Record.Exception(() =>
            {
                var lookup = instanceId switch
                {
                    3 => "DISTRICT",
                    4 => "COUNTY",
                    _ => "JUSTICE"
                };
                var keys = new List<WebNavigationKey> {
                    new() { Name = "StartDate", Value = "2024-05-25"},
                    new() { Name = "EndDate", Value = "2024-05-25"},
                    new() { Name = "CourtType", Value = lookup}
                };
                var wb = new WebNavigationParameter { Keys = keys };
                DallasUiInteractive service = instanceId switch
                {
                    0 => new NoIteratingWeb(wb),
                    1 => new NoIteratingWebWithCancellation(wb),
                    2 => new NoIteratingWebWithNoData(wb),
                    _ => new NoIteratingWeb(wb)
                };
                _ = service.Fetch(CancellationToken.None);
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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ServiceCanIterateDates(int conditionId)
        {
            if (!Debugger.IsAttached) return;
            var error = Record.Exception(() =>
            {
                var keys = new List<WebNavigationKey> {
                    new() { Name = "StartDate", Value = "2024-05-25"},
                    new() { Name = "EndDate", Value = "2024-05-30"},
                    new() { Name = "CourtType", Value = "JUSTICE"}
                };
                var wb = new WebNavigationParameter { Keys = keys };
                var service = new IterateDate(wb);
                service.CheckIteration(conditionId);
            });
            if (conditionId == 4) Assert.Null(error);
            else Assert.NotNull(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ServiceCanIterateItems(int conditionId)
        {
            var error = Record.Exception(() =>
            {
                var keys = new List<WebNavigationKey> {
                    new() { Name = "StartDate", Value = "2024-05-25"},
                    new() { Name = "EndDate", Value = "2024-05-30"},
                    new() { Name = "CourtType", Value = "JUSTICE"}
                };
                var wb = new WebNavigationParameter { Keys = keys };
                var service = new MockIterateItem(wb);
                service.CheckIteration(conditionId);
            });
            if (conditionId == 3) Assert.Null(error);
            else Assert.NotNull(error);
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
            protected override void Iterate(IWebDriver driver, DallasSearchProcess parameters, List<DateTime> dates, List<ICountySearchAction> common, List<ICountySearchAction> postcommon)
            {
                var count = new Faker().Random.Int(10, 20);
                Items.AddRange(itemFaker.Generate(count));
                Items.ForEach(i => People.Add(i.FromDto()));
            }
        }

        private sealed class NoIteratingWebWithCancellation : DallasUiInteractive
        {
            public NoIteratingWebWithCancellation(WebNavigationParameter parameters, bool displayDialogue = true) : base(parameters, displayDialogue)
            {
            }
            public override IWebDriver GetDriver(bool headless = false)
            {
                var mock = new Mock<IWebDriver>();
                return mock.Object;
            }
            protected override void Iterate(IWebDriver driver, DallasSearchProcess parameters, List<DateTime> dates, List<ICountySearchAction> common, List<ICountySearchAction> postcommon)
            {
                var count = new Faker().Random.Int(10, 20);
                Items.AddRange(itemFaker.Generate(count));
                Items.ForEach(i => People.Add(i.FromDto()));
                ExecutionCancelled = true;
            }
        }
        private sealed class NoIteratingWebWithNoData : DallasUiInteractive
        {
            public NoIteratingWebWithNoData(WebNavigationParameter parameters, bool displayDialogue = true) : base(parameters, displayDialogue)
            {
            }
            public override IWebDriver GetDriver(bool headless = false)
            {
                var mock = new Mock<IWebDriver>();
                return mock.Object;
            }
            protected override void Iterate(IWebDriver driver, DallasSearchProcess parameters, List<DateTime> dates, List<ICountySearchAction> common, List<ICountySearchAction> postcommon)
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
                var prc = new DallasSearchProcess();
                List<DateTime> dates = new();
                List<ICountySearchAction> common = new();
                List<ICountySearchAction> postcommon = new();
                if (conditionId == 0) Iterate(null, prc, dates, common, postcommon);
                if (conditionId == 1) Iterate(driver, null, dates, common, postcommon);
                if (conditionId == 2) Iterate(driver, prc, null, common, postcommon);
                if (conditionId == 3) Iterate(driver, prc, dates, null, postcommon);
                if (conditionId == 4) Iterate(driver, prc, dates, common, null);
                if (conditionId == 5) Iterate(driver, prc, dates, common, postcommon);
            }

            protected override void IterateDateRange(IWebDriver driver, DallasSearchProcess parameters, List<DateTime> dates, List<ICountySearchAction> common)
            {
            }

            protected override void IterateItems(IWebDriver driver, DallasSearchProcess parameters, List<ICountySearchAction> postcommon)
            {
            }
        }
        private sealed class IterateDate : DallasUiInteractive
        {
            public IterateDate(WebNavigationParameter parameters, bool displayDialogue = true) : base(parameters, displayDialogue)
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
                var prc = new DallasSearchProcess();
                var mock = new Mock<ICountySearchAction>();
                mock.Setup(a => a.Execute()).Returns(string.Empty);
                List<DateTime> dates = DallasSearchProcess.GetBusinessDays(StartDate, EndingDate);
                List<ICountySearchAction> common = new() {
                    new FakeDallasFetchCaseDetail(),
                    new FakeIntCaseDetail(),
                    new FakeRequestCaptcha(),
                    mock.Object };
                if (conditionId == 0) IterateDateRange(null, prc, dates, common);
                if (conditionId == 1) IterateDateRange(driver, null, dates, common);
                if (conditionId == 2) IterateDateRange(driver, prc, null, common);
                if (conditionId == 3) IterateDateRange(driver, prc, dates, null);
                if (conditionId == 4) IterateDateRange(driver, prc, dates, common);
            }
        }

        private sealed class MockIterateItem : DallasUiInteractive
        {
            public MockIterateItem(WebNavigationParameter parameters, bool displayDialogue = true) : base(parameters, displayDialogue)
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
                var prc = new DallasSearchProcess();
                var mock = new Mock<ICountySearchAction>();
                var count = new Faker().Random.Int(10, 20);
                var items = itemFaker.Generate(count);
                Items.Clear();
                Items.AddRange(items);
                mock.Setup(a => a.Execute()).Returns(string.Empty);
                List<ICountySearchAction> common = new() {
                    new FakeDallasFetchCaseStyle(),
                    new FakeIntCaseStyle(),
                    mock.Object };
                if (conditionId == 0) IterateItems(null, prc, common);
                if (conditionId == 1) IterateItems(driver, null, common);
                if (conditionId == 2) IterateItems(driver, prc, null);
                if (conditionId == 3) IterateItems(driver, prc, common);
            }
        }

        private sealed class FakeDallasFetchCaseDetail : DallasFetchCaseDetail
        {
            public override object Execute()
            {
                var fkr = new Faker();
                var item = fkr.Random.Int(1, 5);
                var list = itemFaker.Generate(item);
                return JsonConvert.SerializeObject(list);
            }
        }

        private sealed class FakeIntCaseDetail : DallasFetchCaseDetail
        {
            public override object Execute()
            {
                var fkr = new Faker();
                var item = fkr.Random.Int(1, 5);
                return item;
            }
        }

        private sealed class FakeDallasFetchCaseStyle : DallasFetchCaseStyle
        {
            public override object Execute()
            {
                var list = caseFaker.Generate();
                return JsonConvert.SerializeObject(list);
            }
        }

        private sealed class FakeRequestCaptcha : DallasRequestCaptcha
        {
            public override object Execute()
            {
                return string.Empty;
            }
        }

        private sealed class FakeIntCaseStyle : DallasFetchCaseStyle
        {
            public override object Execute()
            {
                var fkr = new Faker();
                var item = fkr.Random.Int(1, 5);
                return item;
            }
        }
        private static readonly Faker<CaseItemDto> itemFaker
            = new Faker<CaseItemDto>()
            .RuleFor(x => x.Href, y => y.Internet.Url())
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(16))
            .RuleFor(x => x.FileDate, y => y.Date.Recent().ToString("s").Split('T')[0])
            .RuleFor(x => x.CaseStyle, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.CaseStatus, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.Court, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.CaseType, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.PartyName, y =>
            {
                var fname = y.Name.FirstName();
                var lname = y.Name.LastName();
                return $"{lname}, {fname}".ToUpper();
            })
            .FinishWith((a, b) =>
            {
                b.Plaintiff = a.Company.CompanyName().ToUpper();
                b.CaseStyle = $"{b.Plaintiff} vs. {b.PartyName}";
            });

        private static readonly Faker<DallasCaseStyleDto> caseFaker
            = new Faker<DallasCaseStyleDto>()
            .RuleFor(x => x.CaseStyle, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.Plaintiff, y => y.Random.AlphaNumeric(10));

    }
}
