using LegalLead.PublicData.Search.Helpers;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Interfaces;
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
        public void ServiceCanBeContructed()
        {
            var service = new MocPersistence();
            Assert.NotNull(service);
        }


        [Theory]
        [InlineData("collin")]
        [InlineData("dallas")]
        [InlineData("tarrant")]
        public void ServiceCanUpdateUsageCount(string county)
        {
            var error = Record.Exception(() => {
                var service = new MocPersistence();
                var text = JsonConvert.SerializeObject(GetModel());
                var countyId = county switch
                {
                    "collin" => 10,
                    "tarrant" => 20,
                    _ => 50
                };
                lock (locker)
                {
                    try
                    {
                        service.Write(text);
                        service.UpdateUsageCount(countyId, 15);
                    }
                    finally
                    {
                        service.Initialize();
                    }
                }
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData("collin")]
        [InlineData("dallas")]
        [InlineData("tarrant")]
        public void ServiceCanCheckIsUsageExceeded(string county)
        {
            var error = Record.Exception(() => {
                var service = new MocPersistence();
                var text = JsonConvert.SerializeObject(GetModel());
                var countyId = county switch
                {
                    "collin" => 20,
                    "tarrant" => 10,
                    _ => 60
                };
                lock (locker)
                {
                    try
                    {
                        service.Write(text);
                        var actual = service.IsUsageExceeded(countyId);
                        Assert.False(actual);
                    }
                    finally
                    {
                        service.Initialize();
                    }
                }
            });
            Assert.Null(error);
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
        private static LeadUserSecurityBo GetModel(int monthlyUsage = 100)
        {
            const string tmplist = "collin,tarrant,dallas";
            var oic = StringComparison.OrdinalIgnoreCase;
            var tmps = tmplist.Split(',').ToList(); ;
            var model = ApiAuthenicationService.GetModel(fakeToken, out var _);
            var counties = model.User.CountyData.ToInstance<List<LeadCountyTokenModel>>();
            if (counties.Count > 0)
            {
                counties.ForEach(c => c.MonthlyLimit = monthlyUsage);
                tmps.ForEach(t =>
                {
                    if (!counties.Exists(c => c.CountyName.Equals(t, oic))) {
                        var item = new LeadCountyTokenModel
                        {
                            LeadUserId = counties[0].LeadUserId,
                            CountyName = t,
                            Model = counties[0].Model,
                            MonthlyLimit = monthlyUsage
                        };
                        counties.Add(item);
                    }
                });
            }
            model.User.CountyData = JsonConvert.SerializeObject(counties);
            return model;
        }
        private sealed class MocPersistence : SessionApiFilePersistence
        {
            public MocPersistence()
            {
                var http = new Mock<IHttpService>();
                var mock = new Mock<SessionUsagePersistence>(http.Object);
                mock.Setup(m => m.IncrementUsage(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>())).Returns(true).Verifiable();
                UsagePersistence = mock.Object;
            }
        }
        private static readonly string fakeToken = "MjAyNC0xMS0xN1QwMzo1ODo1OHsiS2V5IjoiZWEwODM3ZmYtNjgxZC00MzIzLWE4NzQtNjJiMTQ3ZjBkNjNmIiwiVXNlciI6eyJJZCI6ImZlZjI5NTMyLWE0ODctMTFlZi05OWNlLTBhZjdhMDFmNTJlOSIsIlVzZXJOYW1lIjoibGVhZC5hZG1pbmlzdHJhdG9yIiwiVXNlckRhdGEiOiJ7XCJVc2VyTmFtZVwiOlwibGVhZC5hZG1pbmlzdHJhdG9yXCIsXCJQaHJhc2VcIjpcInlvdS5jYW4ndC5yZWJvb3QudGhlLmFsYVwiLFwiVmVjdG9yXCI6XCJTNUlvZ3p0cWhNYmkxMmp1TjA1ckZnPT1cIixcIlRva2VuXCI6XCJpaitnTEJ4NWhuaElxenptZll6SlBWTTl6YVptTGtqRnVrN2cvYXY4b3Mvektja3kyQlh1R2JNMjJKSENZbEJvXCIsXCJDcmVhdGVEYXRlXCI6XCIyMDI0LTExLTE3VDAyOjAyOjI3XCIsXCJFbWFpbFwiOlwiZnJhbmsudGhvbXBzb24uanJAZ21haWwuY29tXCIsXCJJbnNlcnRGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl0sXCJVcGRhdGVGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl0sXCJJZFwiOlwiZmVmMjk1MzItYTQ4Ny0xMWVmLTk5Y2UtMGFmN2EwMWY1MmU5XCIsXCJUYWJsZU5hbWVcIjpcIkxFQURVU0VSXCIsXCJGaWVsZExpc3RcIjpbXCJVc2VyTmFtZVwiLFwiUGhyYXNlXCIsXCJWZWN0b3JcIixcIlRva2VuXCIsXCJDcmVhdGVEYXRlXCIsXCJFbWFpbFwiLFwiSWRcIl19IiwiQ291bnR5RGF0YSI6Ilt7XCJMZWFkVXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAxZjUyZTlcIixcIkNvdW50eU5hbWVcIjpcImRhbGxhc1wiLFwiTW9kZWxcIjpcImtlcnJpcGhpbGxpcHNsYXdAZ21haWwuY29tfE1AcmkxMDA3XCJ9XSIsIkluZGV4RGF0YSI6Ilt7XCJMZWFkVXNlcklkXCI6XCJmZWYyOTUzMi1hNDg3LTExZWYtOTljZS0wYWY3YTAxZjUyZTlcIixcIkNvdW50eUxpc3RcIjpcIi0xXCJ9XSJ9LCJSZWFzb24iOiJ1c2VyIGFjY291bnQgYWNjZXNzIGNyZWRlbnRpYWwifQ==";
    }
}
