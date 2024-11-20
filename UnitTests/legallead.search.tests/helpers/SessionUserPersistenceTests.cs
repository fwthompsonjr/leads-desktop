using Bogus;
using LegalLead.PublicData.Search.Helpers;
using Newtonsoft.Json;

namespace legallead.search.tests.helpers
{
    public class SessionUserPersistenceTests
    {
        [Fact]
        public void ServiceCanGetUserName()
        {
            lock (locker)
            {
                var content = GetTempPayload();
                var service = new SessionUserPersistence();
                service.Initialize();
                service.Write(content);
                var actual = service.GetUserName();
                Assert.False(string.IsNullOrEmpty(actual));
            }
        }

        private static string GetTempPayload()
        {
            var faker = new Faker();
            var username = faker.Person.Email;
            var obj = new { UserName = username };
            return JsonConvert.SerializeObject(obj);
        }

        private static readonly object locker = new();
    }
}
