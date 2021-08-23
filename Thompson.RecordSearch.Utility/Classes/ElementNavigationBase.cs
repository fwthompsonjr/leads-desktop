using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public abstract class ElementNavigationBase
    {
        const string element = "Element";

        protected const char comma = ',';

        public virtual string Name
        {
            get
            {
                var typeName = GetType().Name;
                var startAt = element.Length;
                return typeName.Substring(startAt);
            }
        }

        public string StartDate { get; set; }

        public ElementAssertion Assertion { get; set; }
        public abstract IWebElement Execute(WebNavInstruction item);

        protected IWebDriver PageDriver => Assertion?.PageDriver;
    }
}
