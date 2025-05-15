using LegalLead.PublicData.Search.Models;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface IHarrisCivilConfigurationBoProvider
    {
        HarrisCivilConfigurationBo ConfigurationBo { get; }
        string BasePage { get; }

        string GetJs(string key, params object[] args);
    }
}
