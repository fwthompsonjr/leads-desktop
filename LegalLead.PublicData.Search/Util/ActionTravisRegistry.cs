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

            For<ITravisSearchAction>().Add<TravisBeginNavigation>();
            For<ITravisSearchAction>().Add<TravisSetupOptions>();
            For<ITravisSearchAction>().Add<TravisSetupParameters>();
            For<ITravisSearchAction>().Add<TravisNavigateSearch>();
            For<ITravisSearchAction>().Add<TravisFetchCaseItems>();
        }
    }
}