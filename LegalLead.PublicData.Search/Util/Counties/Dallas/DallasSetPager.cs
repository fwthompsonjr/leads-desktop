using LegalLead.PublicData.Search.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasSetPager : BaseDallasSearchAction
    {
        public override int OrderId => 50;
        public bool IsTesting { get; set; }
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            const string noElementId = "ui-tabs-1";
            WaitForTabs(noElementId);
            if (IsNoCount(executor)) return true;
            var casehelper = new DallasSortByCaseTypeHelper(Driver, executor);
            var rowcounter = new DallasGetRecordCountHelper(Driver, executor, js);

            casehelper.Execute();
            rowcounter.Execute();
            return true;
        }

        private void WaitForTabs(string elementId)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(800)
                };
                wait.Until(w => { return w.TryFindElements(By.Id(elementId)) != null; });
            }
            catch
            {
                Debug.WriteLine("Failed to find selector");
            }
        }
        protected override string ScriptName { get; } = "select max rows per page";
    }
}