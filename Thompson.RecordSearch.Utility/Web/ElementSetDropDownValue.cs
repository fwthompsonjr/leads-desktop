// ElementSetDropDownValue
namespace Thompson.RecordSearch.Utility.Web
{
    using System.Threading;
    using Thompson.RecordSearch.Utility.Dto;
    using Byy = OpenQA.Selenium.By;
    using DrpDwn = OpenQA.Selenium.Support.UI.SelectElement;

    public class ElementSetDropDownValue : ElementActionBase
    {
        const string actionName = "set-dropdown-value";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var driver = GetWeb;
            var selector = Byy.CssSelector(item.Locator.Query);
            var elementToClick = driver.FindElement(selector);
            // lets get this item as a SELECT
            var dropDown = new DrpDwn(elementToClick);
            if (string.IsNullOrEmpty(item.DisplayName))
            {
                return;
            }

            var objText = item.ExpectedValue;
            var mxIndex = dropDown.Options.Count - 1;
            var selectedIndex = System.Convert.ToInt32(objText);
            if (selectedIndex > mxIndex)
            {
                selectedIndex = mxIndex;
            }

            dropDown.SelectByIndex(selectedIndex);

            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}
