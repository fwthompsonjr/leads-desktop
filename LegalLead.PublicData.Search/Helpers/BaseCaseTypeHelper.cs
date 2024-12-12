using LegalLead.PublicData.Search.Enumerations;
using LegalLead.PublicData.Search.Interfaces;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Helpers
{
    public abstract class BaseCaseTypeHelper : ICaseTypeIterator
    {
        protected BaseCaseTypeHelper(
            IWebDriver web,
            IJavaScriptExecutor executor)
        {
            Driver = web;
            JsExecutor = executor;
            _ = ParameterList;
        }
        public abstract string Name { get; }
        public abstract int SearchIndex { get; set; }
        public int SearchLimit => ParameterList.Count - 1;
        public IWebDriver Driver { get; set; }
        public IJavaScriptExecutor JsExecutor { get; set; }

        public abstract ExecutionResponseType SetSearchParameter();
        protected abstract List<string> ParameterList { get; }

    }
}
