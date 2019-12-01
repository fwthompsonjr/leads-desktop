using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    using Byy = OpenQA.Selenium.By;

    public class ElementExistsAction : ElementActionBase
    {
        const string actionName = "exists";

        public override string ActionName => actionName;

        public override void Act(Step item)
        {
            var selector = Byy.CssSelector(item.Locator.Query);
            GetAssertion.WaitForElementExist(
                selector,
                string.Format("Looking for {0}", item.DisplayName));
        }
    }
}
