using LegalLead.PublicData.Search.Interfaces;
using StructureMap;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionPersistenceRegistry : Registry
    {
        public SessionPersistenceRegistry()
        {
            For<ISessionPersistance>().Add<SessionFilePersistence>().Named("legacy").Singleton();
            For<ISessionPersistance>().Add<SessionApiFilePersistence>().Named("enhanced").Singleton();
        }
    }
}
