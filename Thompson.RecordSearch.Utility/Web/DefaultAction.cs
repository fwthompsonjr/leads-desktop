using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    public class DefaultAction : ElementActionBase
    {
        const string actionName = "default";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            // do nothing
        }
    }
}
