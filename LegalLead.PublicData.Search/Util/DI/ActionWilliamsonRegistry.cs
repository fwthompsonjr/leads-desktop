using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionWilliamsonRegistry : Registry
    {
        public ActionWilliamsonRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();

            For<ICountySearchAction>().Add<WilliamsonBeginNavigation>().Named("begin");
            For<ICountySearchAction>().Add<WilliamsonNavigateCourtPage>().Named("begin-search");
            For<ICountySearchAction>().Add<WilliamsonSetParameters>().Named("set-parameters");
            For<ICountySearchAction>().Add<HidalgoNoCountVerification>().Named("check-no-count");
            For<ICountySearchAction>().Add<WilliamsonFetchCaseList>().Named("get-case-list");
        }
    }
}