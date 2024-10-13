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

            For<ICountySearchAction>().Add<DallasAuthenicateBegin>().Named("authenticate-step-1");
            For<ICountySearchAction>().Add<DallasAuthenicateSubmit>().Named("authenticate-step-2");
            For<ICountySearchAction>().Add<DallasBeginNavigation>().Named("begin");
            For<ICountySearchAction>().Add<DallasRequestCaptcha>().Named("initialize");
            For<ICountySearchAction>().Add<DallasSetupParameters>().Named("set-parameters");
            For<ICountySearchAction>().Add<DallasNavigateSearch>().Named("perform-search");
            For<ICountySearchAction>().Add<DallasSetPager>().Named("set-max-rows");
            For<ICountySearchAction>().Add<DallasFetchCaseItems>().Named("get-case-list");
            For<ICountySearchAction>().Add<DallasFetchCaseStyle>().Named("get-case-style");
        }
    }
}