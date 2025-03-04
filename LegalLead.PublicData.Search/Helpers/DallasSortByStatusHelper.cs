using LegalLead.PublicData.Search.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;

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
