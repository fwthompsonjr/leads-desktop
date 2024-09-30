using LegalLead.PublicData.Search.Classes;
using OpenQA.Selenium;

namespace LegalLead.PublicData.Search.Util
{
    public interface IDallasAction
    {
        IWebDriver Driver { get; set; }
        DallasAttendedProcess Parameters { get; set; }
        object Execute();
    }
}
