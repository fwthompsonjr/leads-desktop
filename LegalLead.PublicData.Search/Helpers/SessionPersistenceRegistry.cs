﻿using LegalLead.PublicData.Search.Interfaces;
using StructureMap;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionPersistenceRegistry : Registry
    {
        public SessionPersistenceRegistry()
        {
            For<ISessionPersistance>().Add<SessionFilePersistence>().Named("legacy").Singleton();
            For<ISessionPersistance>().Add<SessionApiFilePersistence>().Named("enhanced").Singleton();
            // usage classes
            For<IHttpService>().Add<HttpService>().Singleton();
            For<SessionUsageCapPersistence>().Add<SessionUsageCapPersistence>().Singleton();
            For<SessionUsagePersistence>().Add<SessionUsagePersistence>().Singleton();
            For<SessionUsageReader>().Add<SessionUsageReader>().Singleton();
        }
    }
}