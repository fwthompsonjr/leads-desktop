using LegalLead.PublicData.Search.Helpers;
using Newtonsoft.Json;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace legallead.search.tests.helpers
{
    public class SessionApiFilePersistenceTests
    {
        [Fact]
        public void ServiceCanInitialize()
        {
            lock (locker)
            {
                var service = new SessionApiFilePersistence();
                try
                {
                    service.Initialize();
                    var content = service.Read();
                    Assert.True(string.IsNullOrEmpty(content));
                }
                finally
                {
                    service.Initialize();
                }
            }
        }

        [Fact]
        public void ServiceCanWrite()
        {
            lock (locker)
            {
                var service = new SessionApiFilePersistence();
                try
                {
                    var text = JsonConvert.SerializeObject(GetModel());
                    service.Initialize();
                    service.Write(text);
                    var content = service.Read();
                    var bo = content.ToInstance<LeadUserSecurityBo>();
                    Assert.NotNull(bo);
                }
                finally
                {

                    service.Initialize();
                }
            }
        }

        [Fact]
        public void ServiceCanGetAccountPermissions()
        {
            lock (locker)
            {
                var service = new SessionApiFilePersistence();
                try
                {
                    var text = JsonConvert.SerializeObject(GetModel());
                    service.Initialize();
                    service.Write(text);
                    var content = service.GetAccountPermissions();
                    Assert.False(string.IsNullOrEmpty(content));
                    Assert.Equal("-1", content);
                }
                finally
                {

                    service.Initialize();
                }
            }
        }

        [Theory]
        [InlineData("collin")]
        [InlineData("dallas")]
        [InlineData("tarrant")]
        public void ServiceCanGetAccountCredential(string county)
        {
            lock (locker)
            {
                var service = new SessionApiFilePersistence();
                var expected = !county.Equals("dallas");
                try
                {
                    var text = JsonConvert.SerializeObject(GetModel());
                    service.Initialize();
                    service.Write(text);
                    var content = service.GetAccountCredential(county);
                    var actual = string.IsNullOrEmpty(content);
                    Assert.Equal(expected, actual);
                }
                finally
                {

                    service.Initialize();
                }
            }
        }

        private static readonly object locker = new();
        private static LeadUserSecurityBo GetModel()
        {
            var model = ApiAuthenicationService.GetModel(fakeToken, out var _);
            return model;
        }

        private static readonly string fakeToken = "MjAyNC0xMS0xN1QwMzo1ODo1OHsiS2V5IjoiZWEwODM3ZmYtNjgxZC00MzIzLWE4NzQtNjJiMTQ3ZjBkNjNmIiwiVXNlciI6eyJJZCI6ImZlZjI5NTMyLWE0ODctMTFlZi05OWNlLTBhZjdhMDFmNTJlOSIsIlVzZXJOYW1lIjoibGVhZC5hZG1pbmlzdHJhdG9yIiwiVXNlckRhdGEiOiJ7XCJVc2VyTmFtZVwiOlwibGVhZC5hZG1pbmlzdHJhdG9yXCIsXCJQaHJhc2VcIjpcInlvdS5jYW4ndC5yZWJvb3QudGhlLmFsYVwiLFwiVmVjdG9yXCI6XCJTNUlvZ3p0cWhNYmkxMmp1TjA1ckZnPT1cIixcIlRva2VuXCI6XCJpaitnTEJ4NWhuaElxenptZll6SlBWTTl6YVptTGtqRnVrN2cvYXY4b3Mvektja3kyQlh1R2JNMjJKSENZbEJvXCIsXCJDcmVhdGVEYXRlXCI6XCIyMDI0LTExLTE3VDAyOjAyOjI3XCIsXCJFbWFpbFwiOlwiZnJhbmsudGhvbXBzb24uanJAZ21haWwuY29tXCIsXCJJbnNlcnRGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl0sXCJVcGRhdGVGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl0sXCJJZFwiOlwiZmVmMjk1MzItYTQ4Ny0xMWVmLTk5Y2UtMGFmN2EwMWY1MmU5XCIsXCJUYWJsZU5hbWVcIjpcIkxFQURVU0VSXCIsXCJGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl19IiwiQ291bnR5RGF0YSI6Ilt7XCJMZWFkVXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAxZjUyZTlcIixcIkNvdW50eU5hbWVcIjpcImRhbGxhc1wiLFwiTW9kZWxcIjpcImtlcnJpcGhpbGxpcHNsYXdAZ21haWwuY29tfE1AcmkxMDA3XCJ9XSIsIkluZGV4RGF0YSI6Ilt7XCJMZWFkVXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAxZjUyZTlcIixcIkNvdW50eUxpc3RcIjpcIi0xXCJ9XSJ9LCJSZWFzb24iOiJ1c2VyIGFjY291bnQgYWNjZXNzIGNyZWRlbnRpYWwifQ==";
    }
}
