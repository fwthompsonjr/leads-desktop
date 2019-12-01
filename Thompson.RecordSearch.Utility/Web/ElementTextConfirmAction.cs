using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Web
{
    using Byy = OpenQA.Selenium.By;
    using Thompson.RecordSearch.Utility.Dto;

    public class ElementTextConfirmAction : ElementActionBase
    {
        const string actionName = "text-confirm";

        public override string ActionName => actionName;

        public override void Act(Step item)
        {
            var selector = Byy.CssSelector(item.Locator.Query);
            GetAssertion.MatchText(selector,
                item.DisplayName, item.ExpectedValue);
        }
    }
}
