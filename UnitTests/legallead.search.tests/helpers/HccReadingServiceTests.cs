using Bogus;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Net.Http;
using System.Threading;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.helpers
{
    public class HccReadingServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var provider = GetServices();
            var instance = provider.GetService<IHccReadingService>();
            Assert.NotNull(instance);
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(5)]
        public void ServiceCanFind(int conditionId)
        {
            var provider = GetServices();
            var instance = provider.GetRequiredService<IHccReadingService>();
            var response = conditionId switch
            {
                -1 => null,
                _ => faker.Generate(conditionId)
            };
            var mock = provider.GetRequiredService<Mock<IHttpService>>();
            mock.Setup(m => m.PostAsJson<object, object>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>())).Returns(response);
            _ = instance.Find(DateTime.Now);
            mock.Verify(m => m.PostAsJson<object, object>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()));
        }
        private static IServiceProvider GetServices()
        {
            var services = new ServiceCollection();
            var mock = new Mock<IHttpService>();
            services.AddSingleton(mock);
            services.AddSingleton(mock.Object);
            services.AddSingleton<IHccReadingService, HccReadingService>();
            return services.BuildServiceProvider();
        }
        private static readonly Faker<MockPersonDto> faker =
            new Faker<MockPersonDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CourtDivisionIndicator, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.CaseFileDate, y => y.Date.Past(1).ToString("yyyyMMdd"))
            .RuleFor(x => x.CourtNumber, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CaseStatus, y => y.Random.Guid().ToString())
            .RuleFor(x => x.DefendantStatus, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());
        private sealed class MockPersonDto
        {
            public string Id { get; set; } = string.Empty;
            public string CourtDivisionIndicator { get; set; } = string.Empty;
            public string CaseNumber { get; set; } = string.Empty;
            public string CaseFileDate { get; set; } = string.Empty;
            public string CourtNumber { get; set; } = string.Empty;
            public string CaseStatus { get; set; } = string.Empty;
            public string DefendantStatus { get; set; } = string.Empty;
            public string CurrentOffenseCode { get; set; } = string.Empty;
            public string CurrentOffenseLiteral { get; set; } = string.Empty;
            public string CurrentOffenseLevelAndDegree { get; set; } = string.Empty;
            public string DefendantName { get; set; } = string.Empty;
            public string DefendantStreetNumber { get; set; } = string.Empty;
            public string DefendantStreetName { get; set; } = string.Empty;
            public string DefendantApartmentNumber { get; set; } = string.Empty;
            public string DefendantCity { get; set; } = string.Empty;
            public string DefendantState { get; set; } = string.Empty;
            public string DefendantZip { get; set; } = string.Empty;
            public DateTime? CreateDate { get; set; }
        }
    }
}
