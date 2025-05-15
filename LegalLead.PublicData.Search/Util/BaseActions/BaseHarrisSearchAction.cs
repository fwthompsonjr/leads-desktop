using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    public class BaseHarrisSearchAction : BaseHarrisAction, ICountySearchAction
    {
        public virtual int OrderId => 0;
        public IWebDriver Driver { get; set; }
        public DallasSearchProcess Parameters { get; set; }
        public IWebInteractive Interactive { get; set; }
        public virtual object Execute() { return null; }
    }
}