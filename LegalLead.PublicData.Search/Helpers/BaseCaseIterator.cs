using LegalLead.PublicData.Search.Enumerations;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;

namespace LegalLead.PublicData.Search.Helpers
{
    public abstract class BaseCaseIterator : ICaseTypeIterator
    {

        public abstract string Name { get; }

        public IWebDriver Driver { get; set; }
        public IJavaScriptExecutor JsExecutor { get; set; }

        public int SearchIndex { get; set; } = 0;

        public virtual int SearchLimit => 0;

        public virtual ExecutionResponseType SetSearchParameter()
        {
            return ExecutionResponseType.None;
        }
    }
}
