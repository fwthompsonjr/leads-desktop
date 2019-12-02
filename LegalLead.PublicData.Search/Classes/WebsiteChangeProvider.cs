using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.PublicData.Search.Classes
{
    public class WebsiteChangeProvider
    {
        public WebsiteChangeProvider(FormMain main)
        {
            MainForm = main;
        }

        public FormMain MainForm { get; }


        public IWebsiteChangeEvent GetProvider()
        {
            var providers = ChangeProviders;
            if (providers == null) return null;
            var provider = new Thompson.RecordSearch.Utility.Classes.VersionNameProvider();
            var matched = providers.FirstOrDefault(p => p.Name.Equals(provider.Name));
            if (matched == null) return null;
            matched.GetMain = MainForm;
            return matched;
        }

        protected List<IWebsiteChangeEvent> ChangeProviders
        {
            get
            {
                return _changeProviders ?? (_changeProviders = GetChangeProviders());
            }
        }

        protected List<IWebsiteChangeEvent> GetChangeProviders()
        {
            var type = typeof(IWebsiteChangeEvent);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .ToList();
            var commands = new List<IWebsiteChangeEvent>();
            types.ForEach(f => commands.Add((IWebsiteChangeEvent)Activator.CreateInstance(f)));
            return commands;
        }

        private static List<IWebsiteChangeEvent> _changeProviders;


    }
}
