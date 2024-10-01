using System;
using System.Globalization;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasRequestCaptcha : DallasBaseExecutor
    {
        public override int OrderId => 20;
        public Action PromptUser { get; set; }
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            if (PromptUser == null)
                throw new NullReferenceException(Rx.ERR_DELEGATE_REQUIRED);

            js = VerifyScript(js);
            var result = true;
            while (result)
            {
                result = Convert.ToBoolean(executor.ExecuteScript(js), CultureInfo.CurrentCulture);
                if (result) { PromptUser(); }

            }
            return true;
        }


        protected override string ScriptName { get; } = "is captcha needed";
    }
}