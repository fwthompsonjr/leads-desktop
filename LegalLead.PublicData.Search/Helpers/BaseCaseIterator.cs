using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Enumerations;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Helpers
{
    public abstract class BaseCaseIterator : ICaseTypeIterator
    {

        public abstract string Name { get; }

        public IWebDriver Driver { get; set; }
        public IJavaScriptExecutor JsExecutor { get; set; }

        public int SearchIndex { get; set; } = 0;

        public virtual int SearchLimit => 0;
        public abstract List<DallasJusticeOfficer> Officers { get; }
        public abstract string JsContentScript { get; protected set; }
        public List<CaseTypeExecutionTracker> GetCollection()
        {
            var collection = new List<CaseTypeExecutionTracker>();
            Officers.ForEach(jo => {
                collection.Add(new()
                {
                    Id = Officers.IndexOf(jo),
                    IsExecuted = false,
                    Officer = jo
                });
            });
            return collection;
        }

        public object SetParameter(List<CaseTypeExecutionTracker> collection)
        {
            var selected = collection.Find(x => !x.IsExecuted && x.Officer != null);
            if (selected == null) return null;
            var officer = selected.Officer;
            Console.WriteLine(" - Court location: {0}", officer.Court);
            var js = JsContentScript.Replace("~0", officer.Name);
            var actual = JsExecutor.ExecuteScript(js);
            if (actual is not bool response) return ExecutionResponseType.ExecutionFailed;
            selected.IsExecuted = true;
            return response ? ExecutionResponseType.Success : ExecutionResponseType.ExecutionFailed;
        }

        public virtual ExecutionResponseType SetSearchParameter()
        {
            return ExecutionResponseType.None;
        }
    }
}
