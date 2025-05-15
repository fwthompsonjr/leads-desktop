using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionGraysonRegistry : Registry
    {
        public ActionGraysonRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();

            For<ICountySearchAction>().Add<GraysonBeginNavigation>().Named("begin");
            For<ICountySearchAction>().Add<GraysonNavigateCourtPage>().Named("begin-search");
            For<ICountySearchAction>().Add<GraysonSetParameters>().Named("set-parameters");
            For<ICountySearchAction>().Add<HidalgoNoCountVerification>().Named("check-no-count");
            For<ICountySearchAction>().Add<GraysonFetchCaseList>().Named("get-case-list");
        }
    }
}