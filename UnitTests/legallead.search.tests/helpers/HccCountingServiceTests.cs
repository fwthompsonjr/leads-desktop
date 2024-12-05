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
    public class HccCountingServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var provider = GetServices();
            var instance = provider.GetService<IHccCountingService>();
            Assert.NotNull(instance);
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(5)]
        public void ServiceCanCount(int conditionId)
        {
            var provider = GetServices();
            var instance = provider.GetRequiredService<IHccCountingService>();
            var response = conditionId switch
            {
                -1 => null,
                _ => faker.Generate()
            };
            var mock = provider.GetRequiredService<Mock<IHttpService>>();
            mock.Setup(m => m.PostAsJson<object, object>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>())).Returns(response);
            _ = instance.Count(DateTime.Now);
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
            services.AddSingleton<IHccCountingService, HccCountingService>();
            return services.BuildServiceProvider();
        }
        private static readonly Faker<MockCountDto> faker =
            new Faker<MockCountDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 250000));
        private sealed class MockCountDto
        {
            public string Id { get; set; } = string.Empty;
            public int? RecordCount { get; set; } = null;
        }
    }
}