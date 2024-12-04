using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionHccRegistry : Registry
    {
        public ActionHccRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();
            For<IHccReadingService>().Add<HccReadingService>().Singleton();
            For<IHccWritingService>().Add<HccWritingService>().Singleton();

            For<ICountySearchAction>().Add<HccBeginNavigation>().Named("begin");
            For<ICountySearchAction>().Add<HccDownloadMonthly>().Named("download-monthly");
            For<ICountySearchAction>().Add<HccDownloadSummary>().Named("download-summary");
        }
    }
}