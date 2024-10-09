using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class DallasActionRegistry : Registry
    {
        public DallasActionRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();

            For<IDallasAction>().Add<DallasAuthenicateBegin>().Named("authenticate-step-1");
            For<IDallasAction>().Add<DallasAuthenicateSubmit>().Named("authenticate-step-2");
            For<IDallasAction>().Add<DallasBeginNavigation>().Named("begin");
            For<IDallasAction>().Add<DallasRequestCaptcha>().Named("initialize");
            For<IDallasAction>().Add<DallasSetupParameters>().Named("set-parameters");
            For<IDallasAction>().Add<DallasNavigateSearch>().Named("perform-search");
            For<IDallasAction>().Add<DallasFetchCaseItems>().Named("get-case-list");
            For<IDallasAction>().Add<DallasFetchCaseStyle>().Named("get-case-style");
        }
    }
}