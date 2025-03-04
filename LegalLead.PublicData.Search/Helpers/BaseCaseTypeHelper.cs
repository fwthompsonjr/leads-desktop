using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Enumerations;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Helpers
{
    public abstract class BaseCaseTypeHelper : BaseCaseIterator
    {
        protected BaseCaseTypeHelper(
            IWebDriver web,
            IJavaScriptExecutor executor)
        {
            Driver = web;
            JsExecutor = executor;
            _ = ParameterList;
        }

        public abstract string JsScript { get; protected set; }
        public override string JsContentScript { get; protected set; }
        public override string Name { get; }
        public override int SearchLimit => ParameterList.Count - 1;
        protected abstract List<string> ParameterList { get; }
    }
}
