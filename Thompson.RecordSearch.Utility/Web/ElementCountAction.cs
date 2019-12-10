using System;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    using Byy = OpenQA.Selenium.By;

    public class ElementCountAction : ElementActionBase
    {
        const string actionName = "count";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null) throw new System.ArgumentNullException(nameof(item));
            var driver = GetWeb;
            var selector = Byy.CssSelector(item.Locator.Query);
            if (!int.TryParse(item.ExpectedValue, out int number)) return;
            var matches = driver.FindElements(selector);
            if (matches == null)
            {
                throw new ArgumentOutOfRangeException(item.DisplayName,
                    string.Format(
                    "Expected element collection {0} not found", 
                    item.DisplayName));
            }
            if (matches.Count != number)
            {
                throw new ArgumentOutOfRangeException(item.DisplayName,
                    string.Format(
                    "Expected element count {0} mismatch. Expected {1}, Actual {2}",
                    item.DisplayName,
                    number,
                    matches.Count));
            }
            else
            {
                Console.WriteLine("Expected element count {0} matched. Expected {1}, Actual {2}",
                    item.DisplayName,
                    number,
                    matches.Count);
            }
        }
    }
}
