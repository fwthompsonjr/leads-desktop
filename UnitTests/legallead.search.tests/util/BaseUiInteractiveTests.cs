using Bogus;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace legallead.search.tests.util
{
    public class BaseUiInteractiveTests
    {
        [Fact]
        public void ServiceContainsExpectedProperties()
        {
            var error = Record.Exception(() =>
            {
                var service = new MockUiBase(GetTestParameter());
                Assert.NotEmpty(service.Items);
                Assert.Empty(service.People);
                Assert.Empty(service.Styles);
                Assert.Empty(service.Searches);
                Assert.False(string.IsNullOrEmpty(service.CourtTypeAlias));
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(90)]
        public void ServiceCanGenerateExcelData(int webIndex)
        {
            var error = Record.Exception(() =>
            {
                var service = new MockUiBase(GetTestParameter());
                var actual = service.TestGenerateExcelFile(webIndex);
                Assert.False(string.IsNullOrEmpty(actual));
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ServiceCanSetCompletion(int webIndex)
        {
            var error = Record.Exception(() =>
            {
                var expected = webIndex % 2 == 0;
                var service = new MockUiBase(GetTestParameter());
                var actual = service.SetDateRangeCompletion(expected);
                Assert.Equal(expected, actual);
            });
            Assert.Null(error);
        }

        private static WebNavigationParameter GetTestParameter()
        {
            var websites = new[] { 10, 20, 40, 50, 60, 70, 80 };
            var faker = new Faker();
            var dte = faker.Date.Recent();
            var endDt = dte.AddDays(6);
            return new WebNavigationParameter()
            {
                Id = faker.PickRandom(websites),
                Keys = new() {
                    new() { Name = "StartDate", Value = $"{dte:d}"},
                    new() { Name = "EndDate", Value = $"{endDt:d}"},
                    new() { Name = "CourtType", Value = "JUSTICE"}
                }
            };
        }

        private sealed class MockUiBase : BaseUiInteractive
        {
            public MockUiBase(WebNavigationParameter parameters) : base(parameters)
            {
                var faker = new Faker();
                var count = faker.Random.Int(10, 20);
                Items.AddRange(itemFaker.Generate(count));
            }

            public override WebFetchResult Fetch(CancellationToken token)
            {
                throw new NotImplementedException();
            }

            public bool SetDateRangeCompletion(bool status)
            {
                IsDateRangeComplete = status;
                return IsDateRangeComplete;
            }

            public string TestGenerateExcelFile(int webId = 90)
            {
                People.Clear();
                Items.ForEach(AppendPerson);
                var faker = new Faker().Random.AlphaNumeric(10).ToUpper();
                return GenerateExcelFile(faker, webId, true);
            }

            public string CourtTypeAlias => CourtType;
            public List<CaseItemDto> Styles => CaseStyles;
            public List<ICountySearchAction> Searches => ActionItems;

            protected override string GetCourtAddress(string courtType, string court)
            {
                return string.Empty;
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
                var address = a.Address;
                var addr = $"{address.StreetAddress(true)}|{address.City}, {address.StateAbbr} {address.ZipCode}";
                b.Address = addr;
            });
    }
}
