using LegalLead.PublicData.Search.Helpers;
using Moq;
using Thompson.RecordSearch.Utility.Interfaces;

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
        [InlineData("dallas")]
        public void ServiceCanGetAccountCredential(string county)
        {
            lock (locker)
            {
                var service = new MkFilePersistence();
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


        private sealed class MkFilePersistence : SessionFilePersistence
        {
            public MkFilePersistence()
            {
                const string response = "user-name|abcd1234!";
                var svcs = new Mock<ICountyCodeReader>();
                Reader = svcs.Object;
                svcs.Setup(s => s.GetCountyCode(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(response);
                svcs.Setup(s => s.GetCountyCode(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(response);
            }
        }
        private static readonly string fakeToken = "HJyZFG5dHcmgIIcclPTVatJ2ZfaQi5YJho+8n2cxtWmQyucaQov/GUHhqnFsNqMv";
    }
}