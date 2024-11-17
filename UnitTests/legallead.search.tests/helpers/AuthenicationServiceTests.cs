using LegalLead.PublicData.Search.Helpers;
using Moq;
using System.Net.Http;
using System.Threading;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

namespace legallead.search.tests.helpers
{
    public class AuthenicationServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var error = Record.Exception(() => new MockAuthenicationService());
            Assert.Null(error);
        }


        [Fact]
        public void ServiceHasRetryCount()
        {
            var service = new MockAuthenicationService();
            Assert.Equal(5, service.RetryCount);
        }

        [Theory]
        [InlineData("", "", 0, false)]
        [InlineData("username", "", 0, false)]
        [InlineData("username", "password", 0, true)]
        [InlineData("username", "password", 1, true)]
        [InlineData("username", "password", 10, true)]
        [InlineData("username", "password", -1, false)]
        public void ServiceCanLoginAsync(string uid, string pwd, int index, bool expected)
        {
            var service = new MockAuthenicationService();
            var response = new AuthenicationResponseDto { Id = index };
            var mock = service.TheMock;
            mock.Setup(s => s.PostAsJsonAsync<object, AuthenicationResponseDto>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(response);
            mock.Setup(s => s.PostAsJson<object, AuthenicationResponseDto>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()
            )).Returns(response);
            var actual = service.Login(uid, pwd);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ServiceCanLoginAsyncWithRetries()
        {
            const bool expected = false;
            const string uid = "username";
            const string pwd = "pword";
            const int index = -1;
            int retries = 10;
            var service = new MockAuthenicationService();
            var response = new AuthenicationResponseDto { Id = index };
            var mock = service.TheMock;
            mock.Setup(s => s.PostAsJsonAsync<object, AuthenicationResponseDto>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(response);
            mock.Setup(s => s.PostAsJson<object, AuthenicationResponseDto>(
                It.IsAny<HttpClient>(),
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()
            )).Returns(response);
            while (retries > 0)
            {
                var actual = service.Login(uid, pwd);
                Assert.Equal(expected, actual);
                retries--;
            }
        }

        private sealed class MockAuthenicationService : AuthenicationService
        {
            public Mock<IHttpService> TheMock => MqService;
            private static readonly Mock<IHttpService> MqService = new();

            public MockAuthenicationService() : base(MqService.Object)
            {
            }
        }
    }
}
