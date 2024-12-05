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
    public class HccWritingServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var provider = GetServices();
            var instance = provider.GetService<IHccWritingService>();
            Assert.NotNull(instance);
        }


        [Fact]
        public void ServiceCanWrite()
        {
            var provider = GetServices();
            var instance = provider.GetRequiredService<IHccWritingService>();
            var response = faker.Generate();
            var mock = provider.GetRequiredService<Mock<IHttpService>>();
            mock.Setup(m => m.PostAsJson<object, object>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>())).Returns(response);
            instance.Write(response.Data);
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
            services.AddSingleton<IHccWritingService, HccWritingService>();
            return services.BuildServiceProvider();
        }
        private static readonly Faker<MockCsvDto> faker =
            new Faker<MockCsvDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Data, y => y.Hacker.Phrase())
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 250000));
        private sealed class MockCsvDto
        {
            public string Id { get; set; } = string.Empty;
            public int? RecordCount { get; set; } = null;
            public string Data { get; set; } = string.Empty;
        }
    }
}