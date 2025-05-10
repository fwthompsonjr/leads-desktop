using LegalLead.PublicData.Search.Enumerations;
using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;

namespace LegalLead.PublicData.Search.Helpers
{
    public class TarrantConfigurationBoProvider : ITarrantConfigurationBoProvider
    {
        private readonly TarrantConfigurationBo _bo;
        public TarrantConfigurationBoProvider()
        {
            _bo = JsonConvert.DeserializeObject<TarrantConfigurationBo>(ConfigurationJs) ?? new();
        }
        public TarrantConfigurationBo ConfigurationBo => _bo;
        public string BasePage => ConfigurationBo.Configuration.BasePage;
        public string GetSearchName(TarrantReadMode readMode)
        {
            var id = (int)readMode;
            return ConfigurationBo.Configuration.Links[id];
        }
        public string GetLocationName(int locationId)
        {
            var locations = ConfigurationBo.Configuration.Locations;
            if (locationId < 0 || locationId > locations.Count - 1) return locations[1];
            return locations[locationId];
        }
        public int GetLocationIndex(string locationName)
        {
            int fallbackIndex = 1;
            if (string.IsNullOrEmpty(locationName)) return fallbackIndex;
            var locations = ConfigurationBo.Configuration.Locations;
            var findIndex = locations.FindIndex(x => x.Equals(locationName, StringComparison.OrdinalIgnoreCase));
            if (findIndex < 0) return fallbackIndex;
            return findIndex;
        }
        public string GetJs(string key, params object[] args)
        {
            var scripts = ConfigurationBo.Scripts;
            var requested = scripts.Find(x => x.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (requested == null) return string.Empty;
            var jscontent = string.Join(Environment.NewLine, requested.JsonData);
            if (args.Length <= 0) return jscontent;
            var builder = new StringBuilder(jscontent);
            for (var i = 0; i < args.Length; i++)
            {
                var find = string.Concat("{", $"{i}", "}");
                var replace = Convert.ToString(args[i], CultureInfo.CurrentCulture);
                builder.Replace(find, replace);
            }
            return builder.ToString();
        }
        private static string ConfigurationJs => _configurationJs ??= GetConfigurationJs();
        private static string _configurationJs;
        private static string GetConfigurationJs()
        {
            var content = Properties.Resources.tarrant_scripts_js;
            return Encoding.UTF8.GetString(content);

        }
    }
}
