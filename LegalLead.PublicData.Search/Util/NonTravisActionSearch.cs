using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    public class NonTravisActionSearch : ITravisSearchAction
    {
        public int OrderId => -1;

        public IWebDriver Driver { get; set; }
        public TravisSearchProcess Parameters { get; set; }

        public bool IsPostSearch => false;

        public IWebInteractive Interactive { get; set; }

        public object Execute()
        {
            return null;
        }
    }
}