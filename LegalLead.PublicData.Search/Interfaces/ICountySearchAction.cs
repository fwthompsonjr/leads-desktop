using LegalLead.PublicData.Search.Classes;
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface ICountySearchAction
    {
        int OrderId { get; }
        IWebDriver Driver { get; set; }
        DallasSearchProcess Parameters { get; set; }
        IWebInteractive Interactive { get; set; }
        object Execute();
    }
}
