using LegalLead.PublicData.Search.Interfaces;
using LegalLead.PublicData.Search.Models;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;

namespace LegalLead.PublicData.Search.Helpers
{
    public class HarrisCivilConfigurationBoProvider : IHarrisCivilConfigurationBoProvider
    {
        private readonly HarrisCivilConfigurationBo _bo;
        public HarrisCivilConfigurationBoProvider()
        {
            _bo = JsonConvert.DeserializeObject<HarrisCivilConfigurationBo>(ConfigurationJs) ?? new();
        }
        public HarrisCivilConfigurationBo ConfigurationBo => _bo;
        public string BasePage => ConfigurationBo.Configuration.BasePage;
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
            var content = Properties.Resources.harris_civil_scripts_js;
            return Encoding.UTF8.GetString(content);

        }
    }
}