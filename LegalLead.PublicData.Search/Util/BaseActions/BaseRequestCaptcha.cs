using OpenQA.Selenium;
using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public abstract class BaseRequestCaptcha
    {
        protected abstract IWebDriver WebDriver { get; }
        public Func<bool> PromptUser { get; set; }
        public object GetPromptResponse()
        {
            var executor = GetJavaScriptExecutor();

            if (WebDriver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            if (PromptUser == null)
                throw new NullReferenceException(Rx.ERR_DELEGATE_REQUIRED);

            var retries = 0;
            const int max_retries = 5;
            var result = false;
            while (!result && retries < max_retries)
            {
                result = PromptUser();
                retries++;
            }
            return result;
        }

        public virtual IJavaScriptExecutor GetJavaScriptExecutor()
        {
            return GetExecutor();
        }
        protected IJavaScriptExecutor GetExecutor()
        {
            if (WebDriver is IJavaScriptExecutor exec) return exec;
            return null;
        }
    }
}
