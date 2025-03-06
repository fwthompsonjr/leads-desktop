using LegalLead.PublicData.Search.Util;
using OpenQA.Selenium;

namespace LegalLead.PublicData.Search.Helpers
{
    public class DallasSortByStatusHelper(
        IWebDriver driver,
        IJavaScriptExecutor executor)
    {
        protected readonly IWebDriver Driver = driver;
        protected readonly IJavaScriptExecutor JsExecutor = executor;

        public virtual void Execute()
        {
        }

        protected class NoCountHelper : BaseDallasSearchAction
        {
            public static bool IsNoCountData(IJavaScriptExecutor executor)
            {
                return IsNoCount(executor);
            }
        }

    }
}
