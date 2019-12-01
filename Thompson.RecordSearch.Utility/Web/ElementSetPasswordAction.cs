using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    using Byy = OpenQA.Selenium.By;
    public class ElementSetPasswordAction : ElementActionBase
    {
        const string actionName = "login";

        public override string ActionName => actionName;

        public override void Act(Step item)
        {
            var driver = GetWeb;
            var userDto = UserAccessDto.GetDto(item.ExpectedValue);
            var pwordUser = CryptoEngine.Decrypt(userDto.UserGuid, userDto.UserKey);
            var userId = pwordUser.Split('|');
            var selections = item.Locator.Query.Split('|');
            var idx = 0;
            foreach (var itm in selections)
            {
                var selector = Byy.CssSelector(itm.Trim());
                var elementToClick = driver.FindElement(selector);
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", elementToClick);
                Thread.Sleep(350);
                elementToClick.SendKeys(userId[idx]);
                idx++;
            }
            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }
}
