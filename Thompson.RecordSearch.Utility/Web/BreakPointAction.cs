using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    public class BreakPointAction : ElementActionBase
    {
        const string actionName = "break-point";

        public override string ActionName => actionName;

        public override void Act(Step item)
        {
            System.Diagnostics.Debugger.Break();
        }
    }
}
