using LegalLead.PublicData.Search.Interfaces;
using StructureMap;

namespace LegalLead.PublicData.Search.Util
{
    public class ActionZeroRegistry : Registry
    {
        public ActionZeroRegistry()
        {
            For<ICountySearchAction>().Add<NonActionSearch>();
            For<ITravisSearchAction>().Add<NonTravisActionSearch>();
        }
    }
}