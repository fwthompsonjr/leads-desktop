using Bogus;
using LegalLead.PublicData.Search.Helpers;
using Moq;
using System.Net.Http;
using System.Threading;
using Thompson.RecordSearch.Utility.Interfaces;
using Thompson.RecordSearch.Utility.Models;

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

        [Fact]
        public void ServiceCanExecuteLogin()
        {
            var sut = new MockApiAuthenicationService();
            var faker = new Faker();
            for (int i = 0; i < 5; i++)
            {
                var expected = i == 0;
                var actual = sut.GetService
                    .Login(faker.Person.Email, faker.Random.AlphaNumeric(12));
                Assert.Equal(expected, actual);
            }
        }
        private sealed class MockApiAuthenicationService
        {
            private readonly Mock<IHttpService> httpMock;
            private readonly ApiAuthenicationService service;
            public MockApiAuthenicationService()
            {
                httpMock = new();
                service = new(httpMock.Object);
                var faker = new Faker();
                var userName = faker.Person.Email;
                var response = new ApiResponseModel
                {
                    UserName = userName,
                    Token = fakeToken
                };
                var emptyresponse = new ApiResponseModel
                {
                    UserName = userName,
                    Token = string.Empty
                };
                var unmappableresponse = new ApiResponseModel
                {
                    UserName = userName,
                    Token = "0123456789"
                };
                ApiResponseModel noresponse = null;
                var exception = faker.System.Exception();
                httpMock.SetupSequence(s => s.PostAsJson<object, ApiResponseModel>(
                    It.IsAny<HttpClient>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(response)
                    .Returns(emptyresponse)
                    .Returns(noresponse)
                    .Returns(unmappableresponse)
                    .Throws(exception);
            }

            public Mock<IHttpService> GetHttp => httpMock;
            public ApiAuthenicationService GetService => service;

            private static readonly string fakeToken = "MjAyNC0xMS0xN1QwMzo1ODo1OHsiS2V5IjoiZWEwODM3ZmYtNjgxZC00MzIzLWE4NzQtNjJiMTQ3ZjBkNjNmIiwiVXNlciI6eyJJZCI6ImZlZjI5NTMyLWE0ODctMTFlZi05OWNlLTBhZjdhMDFmNTJlOSIsIlVzZXJOYW1lIjoibGVhZC5hZG1pbmlzdHJhdG9yIiwiVXNlckRhdGEiOiJ7XCJVc2VyTmFtZVwiOlwibGVhZC5hZG1pbmlzdHJhdG9yXCIsXCJQaHJhc2VcIjpcInlvdS5jYW4ndC5yZWJvb3QudGhlLmFsYVwiLFwiVmVjdG9yXCI6XCJTNUlvZ3p0cWhNYmkxMmp1TjA1ckZnPT1cIixcIlRva2VuXCI6XCJpaitnTEJ4NWhuaElxenptZll6SlBWTTl6YVptTGtqRnVrN2cvYXY4b3Mvektja3kyQlh1R2JNMjJKSENZbEJvXCIsXCJDcmVhdGVEYXRlXCI6XCIyMDI0LTExLTE3VDAyOjAyOjI3XCIsXCJFbWFpbFwiOlwiZnJhbmsudGhvbXBzb24uanJAZ21haWwuY29tXCIsXCJJbnNlcnRGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl0sXCJVcGRhdGVGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl0sXCJJZFwiOlwiZmVmMjk1MzItYTQ4Ny0xMWVmLTk5Y2UtMGFmN2EwMWY1MmU5XCIsXCJUYWJsZU5hbWVcIjpcIkxFQURVU0VSXCIsXCJGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl19IiwiQ291bnR5RGF0YSI6Ilt7XCJMZWFkVXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAxZjUyZTlcIixcIkNvdW50eU5hbWVcIjpcImRhbGxhc1wiLFwiTW9kZWxcIjpcImtlcnJpcGhpbGxpcHNsYXdAZ21haWwuY29tfE1AcmkxMDA3XCJ9XSIsIkluZGV4RGF0YSI6Ilt7XCJMZWFkVXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAxZjUyZTlcIixcIkNvdW50eUxpc3RcIjpcIi0xXCJ9XSJ9LCJSZWFzb24iOiJ1c2VyIGFjY291bnQgYWNjZXNzIGNyZWRlbnRpYWwifQ==";
        }
    }
}
