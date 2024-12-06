using Bogus;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Collections.Generic;
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
        private static readonly Faker<MockCsvRecordDto> recfaker =
            new Faker<MockCsvRecordDto>()
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(12))
            .RuleFor(x => x.FilingDate, y => y.Date.Past().ToString("yyyyMMdd"))
            .RuleFor(x => x.Name, y => y.Person.FullName)
            .RuleFor(x => x.StreetNumber, y => y.Random.Int(1, 9999).ToString())
            .RuleFor(x => x.StreetName, y => y.Address.StreetName())
            .RuleFor(x => x.City, y => y.Address.City())
            .RuleFor(x => x.State, y => y.Address.State())
            .RuleFor(x => x.ZipCode, y => y.Address.ZipCode())
            .RuleFor(x => x.Court, y => y.Random.Int(1, 16).ToString("000"));
        private static readonly Faker<MockCsvDto> faker =
            new Faker<MockCsvDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Data, y => y.Hacker.Phrase())
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 250000))
            .FinishWith((f, obj) =>
            {
                var names = "cas,fda,crt,def_nam,def_stnum,def_stnam,def_cty,def_st,def_zip";
                var list = new List<string>
                {
                    string.Join("\t", names.Split(','))
                };
                var dta = recfaker.Generate(300);
                dta.ForEach(d =>
                {
                    var tmp = new[]
                    {
                        d.CaseNumber,
                        d.FilingDate,
                        d.Court,
                        d.Name,
                        d.StreetNumber,
                        d.StreetName,
                        d.City,
                        d.State,
                        d.ZipCode
                    };
                    list.Add(string.Join("\t", tmp));
                });
                obj.Data = string.Join(Environment.NewLine, list);
            });
        private sealed class MockCsvDto
        {
            public string Id { get; set; } = string.Empty;
            public int? RecordCount { get; set; } = null;
            public string Data { get; set; } = string.Empty;
        }

        private sealed class MockCsvRecordDto
        {
            [JsonProperty("cas", Order = 0)] public string CaseNumber { get; set; } = string.Empty;
            [JsonProperty("fda", Order = 1)] public string FilingDate { get; set; } = string.Empty;
            [JsonProperty("crt", Order = 2)] public string Court { get; set; } = string.Empty;
            [JsonProperty("def_nam", Order = 3)] public string Name { get; set; } = string.Empty;
            [JsonProperty("def_stnum", Order = 4)] public string StreetNumber { get; set; } = string.Empty;
            [JsonProperty("def_stnam", Order = 5)] public string StreetName { get; set; } = string.Empty;
            [JsonProperty("def_cty", Order = 6)] public string City { get; set; } = string.Empty;
            [JsonProperty("def_st", Order = 7)] public string State { get; set; } = string.Empty;
            [JsonProperty("def_zip", Order = 8)] public string ZipCode { get; set; } = string.Empty;
        }
    }
}