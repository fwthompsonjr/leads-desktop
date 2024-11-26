using LegalLead.PublicData.Search.Classes;
using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface ITravisSearchAction
    {
        int OrderId { get; }
        bool IsPostSearch { get; }
        IWebDriver Driver { get; set; }
        TravisSearchProcess Parameters { get; set; }
        IWebInteractive Interactive { get; set; }
        object Execute();
    }
}