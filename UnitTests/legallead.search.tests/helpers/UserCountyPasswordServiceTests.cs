using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using Moq;
using System.Net.Http;
using System.Threading;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.helpers
{
    public class UserCountyPasswordServiceTests
    {

        [Fact]
        public void ServiceCanBeContructed()
        {
            var service = new MocPersistence();
            Assert.NotNull(service);
        }

        [Fact]
        public void ServiceCanChangePassword()
        {
            var error = Record.Exception(() =>
            {
                var svc = new MocPersistence();
                var model = new UserCountyPasswordModel();
                var mock = svc.HttpMock;
                mock.Setup(x => x.PostAsJson<UserCountyPasswordModel, object>(
                    It.IsAny<HttpClient>(),
                    It.IsAny<string>(),
                    It.IsAny<UserCountyPasswordModel>(),
                    It.IsAny<CancellationToken>())).Returns(new object());
                svc.Service.ChangePassword(model);
            });
            Assert.Null(error);
        }

        private sealed class MocPersistence
        {
            public MocPersistence()
            {
                HttpMock = new Mock<IHttpService>();
                Service = new UserCountyPasswordService(HttpMock.Object);
            }
            public UserCountyPasswordService Service { get; }
            public Mock<IHttpService> HttpMock { get; }

        }
    }
}