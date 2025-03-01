using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Enumerations;
using LegalLead.PublicData.Search.Helpers;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface ICaseTypeIterator
    {
        string Name { get; }
        IWebDriver Driver { get; set; }
        IJavaScriptExecutor JsExecutor { get; set; }
        ExecutionResponseType SetSearchParameter();
        List<CaseTypeExecutionTracker> GetCollection();
        object SetParameter(List<CaseTypeExecutionTracker> collection);

        int SearchIndex { get; set; }
        int SearchLimit { get; }
        List<DallasJusticeOfficer> Officers { get; }
    }
}
