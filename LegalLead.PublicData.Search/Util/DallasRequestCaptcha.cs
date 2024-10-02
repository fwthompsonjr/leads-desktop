using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasRequestCaptcha : DallasBaseExecutor
    {
        public override int OrderId => 20;
        public Func<bool> PromptUser { get; set; }
        public override object Execute()
        {
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
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


        protected override string ScriptName { get; } = "is captcha needed";
    }
}