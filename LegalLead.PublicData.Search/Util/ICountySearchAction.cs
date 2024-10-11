using LegalLead.PublicData.Search.Classes;
using OpenQA.Selenium;

namespace LegalLead.PublicData.Search.Util
{
    public interface ICountySearchAction
    {
        int OrderId { get; }
        IWebDriver Driver { get; set; }
        DallasAttendedProcess Parameters { get; set; }
        object Execute();
    }
}
