using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionElPasoRegistry : Registry
    {
        public ActionElPasoRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();

            For<ICountySearchAction>().Add<ElPasoBeginNavigation>().Named("begin");
            For<ICountySearchAction>().Add<HidalgoNavigateCourtPage>().Named("begin-search");
            For<ICountySearchAction>().Add<ElPasoSetParameters>().Named("set-parameters");
            For<ICountySearchAction>().Add<HidalgoNoCountVerification>().Named("check-no-count");
            For<ICountySearchAction>().Add<ElPasoFetchCaseList>().Named("get-case-list");
        }
    }
}