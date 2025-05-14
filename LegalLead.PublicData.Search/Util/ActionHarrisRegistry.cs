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
            For<ITarrantConfigurationBoProvider>().Add<TarrantConfigurationBoProvider>().Singleton();

            For<ICountySearchAction>().Add<TarrantBeginNavigation>().Named("begin");
            For<ICountySearchAction>().Add<TarrantSetSearchContext>().Named("begin-search");
            For<ICountySearchAction>().Add<TarrantSetSearchDateParameters>().Named("set-parameters");
            For<ICountySearchAction>().Add<TarrantFetchCaseList>().Named("get-case-list");
            For<ICountySearchAction>().Add<TarrantFetchPersonDetail>().Named("get-person-list");
        }
    }
}