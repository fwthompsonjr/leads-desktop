using OpenQA.Selenium;
using System;
using System.Globalization;
using System.Text;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    using Byy = OpenQA.Selenium.By;
    public class ElementCollinLoginAction : ElementActionBase
    {
        const string actionName = "login-collin-county";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var driver = GetWeb;
            var userDto = UserAccessDto.GetDto(item.ExpectedValue);
            var pwordUser = CryptoEngine.Decrypt(userDto.UserGuid, userDto.UserKey);
            var userId = pwordUser.Split('|');
            var selections = item.Locator.Query.Split('|');
            var script = new StringBuilder();
            var line = Environment.NewLine;
            script.AppendFormat(CultureInfo.CurrentCulture, "document.getElementById('UserName').value = '{0}'{1}", userId[0], line);
            script.AppendFormat(CultureInfo.CurrentCulture, "document.getElementById('Password').value = '{0}'{1}", userId[1], line);

            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript(script.ToString());
            Thread.Sleep(500);


            foreach (var itm in selections)
            {
                var selector = Byy.CssSelector(itm.Trim());
                var elementToClick = driver.FindElement(selector);
                executor.ExecuteScript("arguments[0].focus();", elementToClick);
                Thread.Sleep(300);
                executor.ExecuteScript("arguments[0].blur();", elementToClick);
            }
            executor.ExecuteScript("document.getElementById('Login').submit()");
            driver.WaitForNavigation();
        }
    }
}
