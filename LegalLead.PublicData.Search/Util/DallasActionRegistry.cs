using StructureMap;

namespace LegalLead.PublicData.Search.Util
{
    public class DallasActionRegistry : Registry
    {
        public DallasActionRegistry()
        {
            For<IDallasAction>().Add<DallasBeginNavigation>().Named("begin");
            For<IDallasAction>().Add<DallasRequestCaptcha>().Named("initialize");
            For<IDallasAction>().Add<DallasSetupParameters>().Named("set-parameters");
            For<IDallasAction>().Add<DallasNavigateSearch>().Named("perform-search");
            For<IDallasAction>().Add<DallasFetchCaseItems>().Named("get-case-list");
            For<IDallasAction>().Add<DallasFetchCaseStyle>().Named("get-case-style");
        }
    }
}