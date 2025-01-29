using LegalLead.PublicData.Search.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.Linq;
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
            var helper = new DallasSortByStatusHelper(Driver, executor);
            var casehelper = new DallasSortByCaseTypeHelper(Driver, executor);
            var rowcounter = new DallasGetRecordCountHelper(Driver, executor, js);
            
            helper.Execute();
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
        private bool WaitForSelector()
        {
            try
            {
                Console.WriteLine(" - Waiting for records load to complete");
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20)) { PollingInterval = TimeSpan.FromMilliseconds(400) };
                wait.Until(w => { return w.TryFindElements(By.XPath(_caseLink)) != null; });

                var caseNumbers = Driver.TryFindElements(By.XPath(_caseLink)).ToList();
                if (caseNumbers.Count < 10) { return false; }

                wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30)) { PollingInterval = TimeSpan.FromMilliseconds(400) };
                wait.Until(w => { return w.TryFindElement(By.XPath(_selector)) != null; });
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to find selector");
                return false;
            }
        }
        protected override string ScriptName { get; } = "select max rows per page";
        private const string _selector = "//span[@class='k-pager-sizes k-label']";
        private const string _caseLink = "//a[@class = 'caseLink'][@data-caseid]";
    }
}