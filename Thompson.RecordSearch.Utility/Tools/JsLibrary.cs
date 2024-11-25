using OpenQA.Selenium;

namespace Thompson.RecordSearch.Utility.Tools
{
    internal static class JsLibrary
    {
        public static string GetAttribute(IWebDriver driver, IWebElement element, string attributeName)
        {
            var command = $"return arguments[0].{attributeName}";
            if (!(driver is IJavaScriptExecutor executor)) { return string.Empty; }
            var response = executor.ExecuteScript(command, element);
            if (!(response is string html)) return string.Empty;
            return html;
        }
    }
}
