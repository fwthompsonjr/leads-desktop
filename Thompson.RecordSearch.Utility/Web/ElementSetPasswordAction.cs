using OpenQA.Selenium;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Web
{
    using Byy = OpenQA.Selenium.By;
    public class ElementSetPasswordAction : ElementActionBase
    {
        const string actionName = "login";

        public override string ActionName => actionName;

        public override void Act(NavigationStep item)
        {
            const char pipe = '|';
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var driver = GetWeb;
            var selections = item.Locator.Query.Split('|');
            string[] userId;
            if (item.ExpectedValue.Contains(pipe))
            {
                userId = item.ExpectedValue.Split(pipe);
            }
            else
            {
                var userDto = UserAccessDto.GetDto(item.ExpectedValue);
                var pwordUser = CryptoEngine.Decrypt(userDto.UserGuid, userDto.UserKey);
                userId = pwordUser.Split('|');
            }

            var idx = 0;
            foreach (var itm in selections)
            {
                var selector = Byy.CssSelector(itm.Trim());
                var elementToClick = driver.FindElement(selector);
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                var command = $"arguments[0].value = '{userId[idx]}'";
                executor.ExecuteScript("arguments[0].focus();", elementToClick);
                executor.ExecuteScript(command, elementToClick);
                executor.ExecuteScript("arguments[0].blur();", elementToClick);
                idx++;
            }
            driver.WaitForNavigation();
        }
    }
}
