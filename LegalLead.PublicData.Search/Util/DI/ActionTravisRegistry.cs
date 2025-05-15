using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionTravisRegistry : Registry
    {
        public ActionTravisRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();

            For<ITravisSearchAction>().Add<TravisBeginNavigation>().Named("begin");
            For<ITravisSearchAction>().Add<TravisSetupLocation>().Named("setup-location");
            For<ITravisSearchAction>().Add<TravisSetupOptions>().Named("setup-options");
            For<ITravisSearchAction>().Add<TravisSetupParameters>().Named("setup-parameters");
            For<ITravisSearchAction>().Add<TravisNavigateSearch>().Named("navigate");
            For<ITravisSearchAction>().Add<TravisFetchCaseItems>().Named("fetch-items");
            For<ITravisSearchAction>().Add<TravisFetchClickStyle>().Named("fetch-style");
        }
    }
}