using OpenQA.Selenium;
using System;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class ElementWaitForElementExist : ElementNavigationBase
    {
        public override IWebElement Execute(WebNavInstruction item)
        {
            if (Assertion == null)
            {
                return null;
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.By == CommonKeyIndexes.IdProperCase)
            {
                Assertion.WaitForElementExist(By.Id(item.Value), item.FriendlyName);
            }
            if (item.By == CommonKeyIndexes.XPath)
            {
                Assertion.WaitForElementExist(By.XPath(item.Value), item.FriendlyName);
            }

            return null;
        }
    }
}
