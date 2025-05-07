using LegalLead.PublicData.Search.Enumerations;
using LegalLead.PublicData.Search.Models;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface ITarrantConfigurationBoProvider
    {
        TarrantConfigurationBo ConfigurationBo { get; }
        string BasePage { get; }

        string GetJs(string key, params object[] args);
        string GetLocationName(int locationId);
        string GetSearchName(TarrantReadMode readMode);
    }
}