using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using Moq;
using System.Net.Http;
using System.Threading;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.helpers
{
    public class UserPasswordChangeServiceTests
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
                var model = new UserPasswordChangeModel();
                var mock = svc.HttpMock;
                mock.Setup(x => x.PostAsJson<UserPasswordChangeModel, PasswordChangedResponse>(
                    It.IsAny<HttpClient>(),
                    It.IsAny<string>(),
                    It.IsAny<UserPasswordChangeModel>(),
                    It.IsAny<CancellationToken>())).Returns(new PasswordChangedResponse { Token = "abc" });
                svc.Service.ChangePassword(model);
            });
            Assert.Null(error);
        }

        private sealed class MocPersistence
        {
            public MocPersistence()
            {
                HttpMock = new Mock<IHttpService>();
                Service = new UserPasswordChangeService(HttpMock.Object);
            }
            public UserPasswordChangeService Service { get; }
            public Mock<IHttpService> HttpMock { get; }

        }
    }
}