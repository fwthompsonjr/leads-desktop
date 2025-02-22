﻿using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionHidalgoRegistry : Registry
    {
        public ActionHidalgoRegistry()
        {
            For<IHttpService>().Add<HttpService>().Singleton();
            For<ICountyCodeService>().Add<CountyCodeService>().Singleton();
            For<ICountyCodeReader>().Add<CountyCodeReaderService>().Singleton();

            For<ICountySearchAction>().Add<HidalgoBeginNavigation>().Named("begin");
            For<ICountySearchAction>().Add<HidalgoNavigateCourtPage>().Named("begin-search");
            For<ICountySearchAction>().Add<HidalgoSetParameters>().Named("set-parameters");
            For<ICountySearchAction>().Add<HidalgoNoCountVerification>().Named("check-no-count");
            For<ICountySearchAction>().Add<HidalgoFetchCaseList>().Named("get-case-list");
        }
    }
}