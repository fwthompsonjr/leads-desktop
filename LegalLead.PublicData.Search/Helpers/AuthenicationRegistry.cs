using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class AuthenicationRegistry : Registry
    {
        public AuthenicationRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<IAuthenicationService>().Add<AuthenicationService>().Singleton();
        }
    }
}