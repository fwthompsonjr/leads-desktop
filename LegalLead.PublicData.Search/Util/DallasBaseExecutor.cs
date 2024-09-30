using LegalLead.PublicData.Search.Classes;
using OpenQA.Selenium;
using System.Windows.Media.Animation;
using Thompson.RecordSearch.Utility.Classes;

namespace LegalLead.PublicData.Search.Util
{
    public class DallasBaseExecutor : IDallasAction
    {
        public IWebDriver Driver { get; set; }
        public DallasAttendedProcess Parameters { get; set; }

        public virtual object Execute() { return null; }

        public virtual IJavaScriptExecutor GetJavaScriptExecutor()
        {
            if (Driver is IJavaScriptExecutor exec) return exec;
            return null;
        }


        protected static string GetJsScript(string keyname)
        {
            var obj = DallasScriptHelper.ScriptCollection;
            var exists = obj.TryGetValue(keyname, out var js);
            if (!exists) return string.Empty;
            return js;
        }

    }
}
