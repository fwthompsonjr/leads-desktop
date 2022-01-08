namespace Thompson.RecordSearch.Utility.Web
{
    using OpenQA.Selenium;
    using System.Threading;
    using Thompson.RecordSearch.Utility.Dto;

    public class JquerySetTextBox : ElementActionBase
    {
        const string actionName = "jquery-set-text";

        public override string ActionName => actionName;


        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new System.ArgumentNullException(nameof(item));
            }

            var driver = GetWeb;
            var selector = item.Locator.Query;
            if (string.IsNullOrEmpty(selector))
            {
                return;
            }

            var objText = item.ExpectedValue;
            var command = $"$('{selector}').val('{objText}');";

            var jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript(command);

            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }
    }


}
