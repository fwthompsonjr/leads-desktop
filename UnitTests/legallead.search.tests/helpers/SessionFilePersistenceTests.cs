using LegalLead.PublicData.Search.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace legallead.search.tests.helpers
{
    public class SessionFilePersistenceTests
    {

        [Fact]
        public void ServiceCanGetAccountPermissions()
        {
            lock (locker)
            {
                var service = new SessionFilePersistence();
                try
                {
                    service.Initialize();
                    service.Write(GetModel());
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
                var service = new SessionFilePersistence();
                const bool expected = false;
                try
                {
                    service.Initialize();
                    service.Write(GetModel());
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
        private static string GetModel()
        {
            var txt = fakeToken;
            return txt.Decrypt();
        }

        private static readonly string fakeToken = "HJyZFG5dHcmgIIcclPTVatJ2ZfaQi5YJho+8n2cxtWmQyucaQov/GUHhqnFsNqMv";
    }
}