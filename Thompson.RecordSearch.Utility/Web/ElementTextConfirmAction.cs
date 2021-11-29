namespace Thompson.RecordSearch.Utility.Web
{
    using Thompson.RecordSearch.Utility.Dto;
    using Byy = OpenQA.Selenium.By;

    public class ElementTextConfirmAction : ElementActionBase
    {
        const string actionName = "text-confirm";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var selector = Byy.CssSelector(item.Locator.Query);
            GetAssertion.MatchText(selector,
                item.DisplayName, item.ExpectedValue);
        }
    }
}
