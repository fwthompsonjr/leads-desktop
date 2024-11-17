using LegalLead.PublicData.Search.Helpers;
using Moq;
using Thompson.RecordSearch.Utility.Interfaces;

namespace legallead.search.tests.helpers
{
    public class ApiAuthenicationServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var sut = new MockApiAuthenicationService();
            Assert.NotNull(sut.GetService);
            Assert.NotNull(sut.GetHttp);
        }

        private sealed class MockApiAuthenicationService
        {
            private readonly Mock<IHttpService> httpMock;
            private readonly ApiAuthenicationService service;
            public MockApiAuthenicationService()
            {
                httpMock = new();
                service = new(httpMock.Object);
            }

            public Mock<IHttpService> GetHttp => httpMock;
            public ApiAuthenicationService GetService => service;
        }
    }
}
