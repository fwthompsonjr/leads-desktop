using LegalLead.PublicData.Search.Classes;
using OpenQA.Selenium;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface ICountySearchAction
    {
        int OrderId { get; }
        IWebDriver Driver { get; set; }
        DallasAttendedProcess Parameters { get; set; }
        object Execute();
    }
}
