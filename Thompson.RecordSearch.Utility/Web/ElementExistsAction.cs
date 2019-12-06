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
            if (item == null) throw new System.ArgumentNullException(nameof(item));
            var selector = Byy.CssSelector(item.Locator.Query);
            GetAssertion.WaitForElementExist(
                selector,
                string.Format("Looking for {0}", item.DisplayName));
        }
    }
}
