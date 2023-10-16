using System;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    public class GetRecordCountAction : ElementActionBase
    {
        const string actionName = "get-record-count";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            var selector = GetSelector(item);
            var element = GetWeb.TryFindElement(selector);
            if (element != null)
            {
                Console.WriteLine("Search found {0} records.", element.Text);
            }

        }
    }
}
