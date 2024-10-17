using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class TravisSetupAlternateParameters : BaseTravisSearchAction
    {
        public override int OrderId => 30;
        public int CourtLocationId { get; set; } = -1;
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

            if (Parameters.CourtLocator == null || Parameters.CourtLocator.Count == 0)
                throw new NullReferenceException(Rx.ERR_COURT_TYPE_MISSING);

            var count = Parameters.CourtLocator.Count - 1;
            if (CourtLocationId < 0 || CourtLocationId > count)
                throw new NullReferenceException(Rx.ERR_COURT_TYPE_MISSING);

            if (string.IsNullOrEmpty(Parameters.CourtLocator[CourtLocationId]))
                throw new NullReferenceException(Rx.ERR_COURT_TYPE_MISSING);

            js = VerifyScript(js);
            var script = js.Replace("{0}", Parameters.StartDate)
                .Replace("{1}", Parameters.EndingDate)
                .Replace("{2}", Parameters.CourtLocator[CourtLocationId]);

            executor.ExecuteScript(script);
            return true;
        }

        protected override string ScriptName { get; } = "set alternate search parms";
    }
}