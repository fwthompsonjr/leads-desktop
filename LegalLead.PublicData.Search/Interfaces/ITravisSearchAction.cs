using LegalLead.PublicData.Search.Classes;
using OpenQA.Selenium;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface ITravisSearchAction
    {
        int OrderId { get; }
        bool IsPostSearch { get; }
        IWebDriver Driver { get; set; }
        TravisSearchProcess Parameters { get; set; }
        object Execute();
    }
}