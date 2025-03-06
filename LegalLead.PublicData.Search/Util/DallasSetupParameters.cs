using LegalLead.PublicData.Search.Extensions;
using System;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class DallasSetupParameters : BaseDallasSearchAction
    {
        public override int OrderId => 30;
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

            if (string.IsNullOrEmpty(Parameters.CourtLocator))
                throw new NullReferenceException(Rx.ERR_COURT_TYPE_MISSING);

            js = VerifyScript(js);
            var script = js.Replace("{0}", Parameters.StartDate)
                .Replace("{1}", Parameters.EndingDate)
                .Replace("{2}", Parameters.CourtLocator);
            var arr = new string[] { StatusScript, script };
            var cmmd = string.Join(Environment.NewLine, arr);
            Driver.ExecuteScriptWithRetry(executor, TimeSpan.FromSeconds(5), cmmd);
            return true;
        }

        protected override string ScriptName { get; } = "set start and end date";
        private static string StatusScript
        {
            get
            {
                if (!string.IsNullOrEmpty(setStatusScript)) return setStatusScript;
                setStatusScript = string.Join(Environment.NewLine, setStatusBlock);
                return setStatusScript;
            }
        }

        private static string setStatusScript = null;

        private static readonly string[] setStatusBlock = new string[] {
            "",
            "var sts = { ",
            " 'set_status': function() { ",
            "	try { ",
            "		var combobox = $('#caseCriteria_CaseStatus').data('kendoComboBox'); ",
            "		combobox.value('OPEN'); ",
            "	} catch { ",
            "	  // intentionally left blank ",
            "	} ",
            " } ",
            "}",
            "sts.set_status();"
        };
    }
}