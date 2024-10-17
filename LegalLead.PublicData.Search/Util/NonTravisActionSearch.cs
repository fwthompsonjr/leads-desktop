using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;

namespace LegalLead.PublicData.Search.Util
{
    public class NonTravisActionSearch : ITravisSearchAction
    {
        public int OrderId => -1;

        public IWebDriver Driver { get; set; }
        public TravisSearchProcess Parameters { get; set; }

        public bool IsPostSearch => false;

        public object Execute()
        {
            return null;
        }
    }
}