using Bogus;
using LegalLead.PublicData.Search.Helpers;
using System.Diagnostics;

namespace legallead.search.tests.helpers
{
    public class SessionUtilTests
    {
        [Fact]
        public void ServiceCanEncodeString()
        {
            var expected = new Faker().Hacker.Phrase();
            string a = expected.Crypt();
            Debug.WriteLine(a);
            string actual = a.Decrypt();
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ServiceCanInitialize()
        {
            var error = Record.Exception(() => {
                SessionUtil.Initialize();
            });
            Assert.Null(error);
        }
    }
}
