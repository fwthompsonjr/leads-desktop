using OpenQA.Selenium;
using System;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class ElementGetElement : ElementNavigationBase
    {
        public override IWebElement Execute(WebNavInstruction item)
        {
            if (Assertion == null)
            {
                return null;
            }

            if (PageDriver == null)
            {
                return null;
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (item.By == CommonKeyIndexes.IdProperCase)
            {
                return PageDriver.FindElement(By.Id(item.Value));
            }
            if (item.By == CommonKeyIndexes.XPath)
            {
                return PageDriver.FindElement(By.XPath(item.Value));
            }
            return null;
        }
    }
}
