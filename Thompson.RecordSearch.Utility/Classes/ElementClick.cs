using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class ElementClick : ElementNavigationBase
    {
        public override IWebElement Execute(WebNavInstruction item)
        {
            if (Assertion == null) return null;
            if (PageDriver == null) return null;
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (item.By == CommonKeyIndexes.IdProperCase)
            {
                PageDriver.FindElement(By.Id(item.Value)).Click();
            }
            if (item.By == CommonKeyIndexes.XPath)
            {
                PageDriver.FindElement(By.XPath(item.Value)).Click();
            }
            return null;
        }
    }
}
