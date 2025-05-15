using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarSetupParameters : BaseBexarSearchAction
    {
        public override int OrderId => 30;
        public override object Execute()
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
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
            var courtIndex = "4";
            if (Parameters.CourtType.Equals("County", oic)) courtIndex = "2";
            if (Parameters.CourtType.Equals("District", oic)) courtIndex = "3";
            js = VerifyScript(js);

            var script = js.Replace("{1}", Parameters.StartDate)
                .Replace("{2}", Parameters.EndingDate)
                .Replace("{0}", courtIndex);
            executor.ExecuteScript(script);
            WaitForNavigation();
            return true;
        }
        protected override string ScriptName { get; } = "set search parameters and submit";
    }
}