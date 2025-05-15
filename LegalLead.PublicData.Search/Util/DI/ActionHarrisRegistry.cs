using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionHarrisRegistry : Registry
    {
        public ActionHarrisRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();
            For<IHarrisCivilConfigurationBoProvider>().Add<HarrisCivilConfigurationBoProvider>().Singleton();

            For<ICountySearchAction>().Add<HarrisBeginNavigation>().Named("begin");
            For<ICountySearchAction>().Add<HarrisSetSearchContext>().Named("begin-search");
            For<ICountySearchAction>().Add<HarrisSetSearchDateParameters>().Named("set-parameters");
            For<ICountySearchAction>().Add<HarrisSubmitSearch>().Named("submit-form");
            For<ICountySearchAction>().Add<HarrisGetRecordCount>().Named("get-record-count");
            For<ICountySearchAction>().Add<HarrisFetchPersonDetail>().Named("get-person-list");
        }
    }
}