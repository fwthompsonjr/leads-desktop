using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasRequestCaptcha : DallasBaseExecutor
    {
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

        [ExcludeFromCodeCoverage]
        private static string VerifyScript(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException(Rx.ERR_SCRIPT_MISSING);
            return source;
        }

        private static string jsContent = null;
        private static string JsScript
        {
            get
            {
                if (!string.IsNullOrEmpty(jsContent)) return jsContent;
                jsContent = GetJsScript(sourceScript);
                return jsContent;
            }
        }
        private const string sourceScript = "is captcha needed";
    }
}