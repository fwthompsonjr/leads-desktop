using OpenQA.Selenium;
using System;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class ElementClickElement : ElementNavigationBase
    {
        public override IWebElement Execute(WebNavInstruction item)
        {
            if (Assertion == null) return null;
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            Assertion.ClickElement(item.Value);
            return null;
        }
    }
}
