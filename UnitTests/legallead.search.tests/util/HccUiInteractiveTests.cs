using Bogus;
using LegalLead.PublicData.Search.Util;
using Moq;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace legallead.search.tests.util
{
    public class HccUiInteractiveTests
    {
        [Fact]
        public void ServiceHasTypeDefined()
        {
            var error = Record.Exception(() => _ = typeof(HccUiInteractive));
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        [InlineData(true, false)]
        public void ServiceCanBeConstructed(bool allowDownload, bool isTest)
        {
            var parameter = GetParameter();
            var error = Record.Exception(() => _ = new MockHccUiInteractive(parameter, allowDownload, isTest));
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServiceCanFetch(bool useMockData)
        {
            const bool allowDownload = false;
            const bool isTest = true;
            var parameter = GetParameter();
            var error = Record.Exception(() =>
            {
                var svc = new MockHccUiInteractive(parameter, allowDownload, isTest, useMockData);
                _ = svc.Fetch();
            });
            Assert.Null(error);
        }

        private static readonly Faker<CaseItemDto> faker
            = new Faker<CaseItemDto>()
            .RuleFor(x => x.FileDate, y => y.Date.Recent().ToString("d"))
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(10).ToUpper());
        private static WebNavigationParameter GetParameter()
        {
            var keys = new List<WebNavigationKey> {
                new() { Name = "StartDate", Value = "2024-05-25"},
                new() { Name = "EndDate", Value = "2024-05-28"},
                new() { Name = "CourtType", Value = "JUSTICE"}
            };
            return new WebNavigationParameter { Keys = keys };
        }
        private sealed class MockHccUiInteractive : HccUiInteractive
        {
            public MockHccUiInteractive(
                WebNavigationParameter parameters,
                bool allowDownload = true,
                bool isTestMode = false,
                bool useCaseList = true) : base(parameters, allowDownload, isTestMode)
            {
                if (useCaseList) return;
                var find = ActionItems.Find(x => x.GetType() == typeof(HccFetchCaseList));
                if (find == null) return;
                var responses = new[]
                {
                    faker.Generate(2).ToJsonString(),
                    faker.Generate(5).ToJsonString(),
                    faker.Generate(10).ToJsonString(),
                    faker.Generate(6).ToJsonString(),
                    faker.Generate(8).ToJsonString(),
                    faker.Generate(8).ToJsonString(),
                };
                var mock = new Mock<HccFetchCaseList>();
                mock.SetupSequence(x => x.Execute())
                    .Returns(responses[0])
                    .Returns(responses[1])
                    .Returns(responses[2])
                    .Returns(responses[3])
                    .Returns(responses[4])
                    .Returns(responses[5]);
                var id = ActionItems.IndexOf(find);
                ActionItems[id] = mock.Object;

            }
        }
    }
}