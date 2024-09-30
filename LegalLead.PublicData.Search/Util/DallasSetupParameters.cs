using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasSetupParameters : DallasBaseExecutor
    {
        public override object Execute()
        {
            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            if (string.IsNullOrEmpty(Parameters.StartDate))
                throw new NullReferenceException(Rx.ERR_START_DATE_MISSING);

            if (string.IsNullOrEmpty(Parameters.EndingDate))
                throw new NullReferenceException(Rx.ERR_END_DATE_MISSING);

            if (string.IsNullOrEmpty(Parameters.CourtType))
                throw new NullReferenceException(Rx.ERR_COURT_TYPE_MISSING);

            js = VerifyScript(js);
            var script = js.Replace("{0}", Parameters.StartDate)
                .Replace("{1}", Parameters.EndingDate)
                .Replace("{2}", Parameters.CourtType);
            executor.ExecuteScript(script);
            return true;
        }

        protected override string ScriptName { get; } = "set start and end date";
    }
}