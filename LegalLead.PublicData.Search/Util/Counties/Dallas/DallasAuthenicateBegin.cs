using LegalLead.PublicData.Search.Common;
using LegalLead.PublicData.Search.Helpers;
using LegalLead.PublicData.Search.Interfaces;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>")]
    public class DallasAuthenicateBegin : BaseDallasSearchAction
    {
        protected readonly ICountyCodeReader _reader;
        public DallasAuthenicateBegin(ICountyCodeReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            SessionPersistance = SessionPersistenceContainer.GetContainer
                .GetInstance<ISessionPersistance>(ApiHelper.ApiMode);
        }
        public override int OrderId => 4;
        public override object Execute()
        {

            if (string.IsNullOrEmpty(_credential))
                _credential = SessionPersistance.GetAccountCredential("dallas");

            var js = JsScript;
            var executor = GetJavaScriptExecutor();

            var destination = NavigationUri;

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            Uri uri = GetUri(destination);
            Driver.Navigate().GoToUrl(uri);
            if (string.IsNullOrEmpty(_credential)) return false;

            js = VerifyScript(js);
            ExecuteScriptWithWait(Driver, executor, js);
            Thread.Sleep(1000);
            if (IsCaptchaRequested(Driver))
            {
                var handleCaptcha = DisplayUserCaptchaHelper.UserPrompt();
                if (!handleCaptcha) return false;
            }
            return FindUserName();
        }

        private bool FindUserName()
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(250),
                };
                wait.Until(d =>
                {
                    var uid = d.TryFindElement(By.Id("UserName"));
                    return uid != null;
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected string _credential;

        [ExcludeFromCodeCoverage]
        protected static Uri GetUri(string destination)
        {
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentException(Rx.ERR_URI_MISSING);
            if (!Uri.TryCreate(destination, UriKind.Absolute, out var uri))
                throw new ArgumentException(Rx.ERR_URI_MISSING);
            return uri;
        }

        private static string navigationUri = null;
        private static string NavigationUri
        {
            get
            {
                if (!string.IsNullOrEmpty(navigationUri)) return navigationUri;
                navigationUri = GetNavigationUri();
                return navigationUri;
            }
        }

        private static string GetNavigationUri()
        {
            var obj = DallasScriptHelper.NavigationSteps;
            var item = obj.Find(x => x.ActionName == "navigate");
            if (item == null) return string.Empty;
            return item.Locator.Query;
        }

        protected bool IsCaptchaRequested(IWebDriver driver)
        {
            if (driver is not IJavaScriptExecutor exec) return false;
            var response = exec.ExecuteScript(HumanScriptJs);
            if (response is not string result) return false;
            var model = JsonConvert.DeserializeObject<HumanResponseModel>(result);
            if (model is null) return false;
            return model.HasCaptcha;
        }

        protected override string ScriptName { get; } = "login process 01";
        protected ISessionPersistance SessionPersistance { get; set; }


        private bool ExecuteScriptWithWait(IWebDriver driver, IJavaScriptExecutor exec, string jscript)
        {
            if (string.IsNullOrWhiteSpace(jscript)) return false;
            var currentUri = driver.Url;
            try
            {
                exec.ExecuteScript(jscript);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500),
                };
                wait.Until(d => !d.Url.Equals(currentUri, StringComparison.OrdinalIgnoreCase));
                driver.WaitForDocumentReady(exec, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(500));
                return true;
            }
            catch
            {
                return false;
            }
        }


        protected readonly ITarrantConfigurationBoProvider BoProvider
            = ActionTarrantContainer.GetContainer.GetInstance<ITarrantConfigurationBoProvider>();
        private const string HumanScriptName = "is-human-page";
        private static string humanScript;
        protected string HumanScriptJs => humanScript ??= GetHumanScriptJs();
        private string GetHumanScriptJs()
        {
            return BoProvider.GetJs(HumanScriptName);
        }
        private sealed class HumanResponseModel
        {
            [JsonProperty("hasCaptcha")] public bool HasCaptcha { get; set; }
        }
    }
}