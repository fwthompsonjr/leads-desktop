using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    public class NonActionSearch : ICountySearchAction
    {
        public int OrderId => -1;

        public IWebDriver Driver { get; set; }
        public DallasSearchProcess Parameters { get; set; }
        public IWebInteractive Interactive { get; set; }
        public object Execute()
        {
            return null;
        }
    }
}
