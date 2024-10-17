using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;

namespace LegalLead.PublicData.Search.Util
{
    public class NonActionSearch : ICountySearchAction
    {
        public int OrderId => -1;

        public IWebDriver Driver { get; set; }
        public DallasSearchProcess Parameters { get; set; }

        public object Execute()
        {
            return null;
        }
    }
}
