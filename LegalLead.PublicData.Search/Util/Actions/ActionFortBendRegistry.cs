using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionFortBendRegistry : Registry
    {
        public ActionFortBendRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();

            For<ICountySearchAction>().Add<FortBendBeginNavigation>().Named("begin");
            For<ICountySearchAction>().Add<HidalgoNavigateCourtPage>().Named("begin-search");
            For<ICountySearchAction>().Add<FortBendSetParameters>().Named("set-parameters");
            For<ICountySearchAction>().Add<HidalgoNoCountVerification>().Named("check-no-count");
            For<ICountySearchAction>().Add<FortBendFetchCaseList>().Named("get-case-list");
            For<ICountySearchAction>().Add<FortBendFetchClickStyle>().Named("get-person-list");
        }
    }
}