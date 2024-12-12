using LegalLead.PublicData.Search.Enumerations;
using OpenQA.Selenium;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface ICaseTypeIterator
    {
        string Name { get; }
        IWebDriver Driver { get; set; }
        IJavaScriptExecutor JsExecutor { get; set; }
        ExecutionResponseType SetSearchParameter();
        int SearchIndex { get; set; }
        int SearchLimit { get; }
    }
}
