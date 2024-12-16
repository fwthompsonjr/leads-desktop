using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

namespace legallead.search.tests.helpers
{
    public class SessionUsageReaderTests
    {
        [Fact]
        public void ServiceCanBeConstructed()
        {
            var sut = new MkPersistence();
            Assert.NotNull(sut);
            Assert.NotNull(sut.MqHttp);
            Assert.NotNull(sut.Service);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(5)]
        public void ServiceCanGetUsage(int testIndex)
        {
            var sut = new MkPersistence();
            var mock = sut.MqHttp;
            var service = sut.Service;
            DateTime? date = testIndex == 1 ? null : DateTime.Now;
            List<UsageHistoryModel> response = testIndex == 4 ? null : new();
            mock.Setup(m => m.PostAsJson<object, List<UsageHistoryModel>>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>())).Returns(response);
            var actual = service.GetUsage(date);
            Assert.NotNull(actual);
        }

        private sealed class MkPersistence
        {
            public MkPersistence()
            {
                MqHttp = new Mock<IHttpService>();
                MqDb = new Mock<IRemoteDbHelper>();
                Service = new SessionUsageReader(MqHttp.Object, MqDb.Object);
            }

            public Mock<IRemoteDbHelper> MqDb { get; }
            public Mock<IHttpService> MqHttp { get; }
            public SessionUsageReader Service { get; }
        }
    }
}