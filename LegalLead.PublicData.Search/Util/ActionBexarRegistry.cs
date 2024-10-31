using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionBexarRegistry : Registry
    {
        public ActionBexarRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();

            For<ICountySearchAction>().Add<BexarBeginNavigation>().Named("begin");
            For<ICountySearchAction>().Add<BexarSetupParameters>().Named("set-parameters");
            For<ICountySearchAction>().Add<BexarSetNoCountVerification>().Named("check-no-count");
            For<ICountySearchAction>().Add<BexarSetPager>().Named("set-max-rows");
            For<ICountySearchAction>().Add<BexarSetPagerVerification>().Named("set-max-rows-verification");
            For<ICountySearchAction>().Add<BexarFetchCaseDetail>().Named("get-case-list");
            For<ICountySearchAction>().Add<BexarFetchFilingDetail>().Named("get-filing-list");
        }
    }
}