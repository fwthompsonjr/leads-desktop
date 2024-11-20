using LegalLead.PublicData.Search.Helpers;
using Moq;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.helpers
{
    public class SessionUsagePersistenceTests
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
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void ServiceCanIncrementUsage(int testIndex)
        {
            var error = Record.Exception(() =>
            {

                try
                {
                    var sessionitem = testIndex == 5 ? string.Empty : "temp string from unit test";
                    SessionUtil.Write(sessionitem);
                    var sut = new MkPersistence();
                    var mock = sut.MqHttp;
                    var service = sut.Service;
                    var request = new
                    {
                        userid = testIndex == 1 ? string.Empty : "012345678",
                        county = testIndex == 2 ? string.Empty : "testcounty",
                        recordCount = testIndex == 3 ? -10 : 50
                    };
                    object response = testIndex == 4 ? null : new();
                    mock.Setup(m => m.PostAsJson<object, object>(
                        It.IsAny<HttpClient>(),
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        It.IsAny<CancellationToken>())).Returns(response);
                    var expected = testIndex == 0;
                    var actual = service.IncrementUsage(request.userid, request.county, request.recordCount);
                    Debug.WriteLine("Test : {0}. Expected: {1}. Actual {2}",
                        testIndex,
                        expected,
                        actual);
                }
                finally
                {
                    SessionUtil.Initialize();
                }
            });
            Assert.Null(error);
        }

        private sealed class MkPersistence
        {
            public MkPersistence()
            {
                MqHttp = new Mock<IHttpService>();
                Service = new SessionUsageCapPersistence(MqHttp.Object);
            }

            public Mock<IHttpService> MqHttp { get; }
            public SessionUsageCapPersistence Service { get; }
        }
    }
}