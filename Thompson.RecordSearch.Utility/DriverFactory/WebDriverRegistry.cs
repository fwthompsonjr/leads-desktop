
using StructureMap;

namespace Thompson.RecordSearch.Utility.DriverFactory
{
    public class WebDriverRegistry : Registry
    {
        public WebDriverRegistry()
        {
            For<IWebDriverProvider>().Use<ChromeProvider>();

            For<IWebDriverProvider>().Add<ChromeProvider>().Named("Chrome");
            For<IWebDriverProvider>().Add<ChromeOlderProvider>().Named("Chrome Legacy");
            For<IWebDriverProvider>().Add<FireFoxProvider>().Named("Firefox");
            For<IWebDriverProvider>().Add<EdgeProvider>().Named("Edge");
        }
    }
}