using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionTravisAlternateRegistry : Registry
    {
        public ActionTravisAlternateRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();

            For<ITravisSearchAction>().Add<TravisBeginNavigation>().Named("begin");
            For<ITravisSearchAction>().Add<TravisRequestCaptcha>().Named("initialize");
            For<ITravisSearchAction>().Add<TravisSetupAlternateParameters>().Named("set-parameters");
            For<ITravisSearchAction>().Add<TravisNavigateAlternateSearch>().Named("perform-search");
            For<ITravisSearchAction>().Add<TravisSetPager>().Named("set-max-rows");
            For<ITravisSearchAction>().Add<TravisFetchAlternateCaseItems>().Named("get-case-list");
            For<ITravisSearchAction>().Add<TravisFetchAlternateCaseStyle>().Named("get-case-style");
        }
    }
}