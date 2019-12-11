using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class ElementSetControlValue : ElementNavigationBase
    {
        public override IWebElement Execute(WebNavInstruction item)
        {
            if (Assertion == null) return null;
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            var indexes = item.Value.Split(comma);
            var idx = indexes[0];
            var txt = indexes[1];
            if (item.FriendlyName.Equals(CommonKeyIndexes.DateFiledOnTextBox,
                StringComparison.CurrentCultureIgnoreCase))
            {
                txt = StartDate;
            }
            Assertion.ControlSetValue(idx, txt);
            return null;
            throw new NotImplementedException();
        }
    }
}
