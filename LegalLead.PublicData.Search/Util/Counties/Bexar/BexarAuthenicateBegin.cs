using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Interfaces;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarAuthenicateBegin : DallasAuthenicateBegin
    {
        public BexarAuthenicateBegin(ICountyCodeReader reader) : base(reader) { }
        protected virtual string CountyName => "bexar";
        public override int OrderId => 14;
        public string LoginAddress { get; set; } = string.Empty;
        public bool IsLoginRequested { get; set; } = true;
        public override object Execute()
        {
            const char pipe = '|';
            if (!IsLoginRequested) { return false; }
            if (string.IsNullOrEmpty(_credential))
                _credential = SessionPersistance.GetAccountCredential(CountyName);

            var executor = GetJavaScriptExecutor();
            var destination = LoginAddress;

            if (Parameters == null || Driver == null || executor == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            var isNavigationNeeded = false;
            var returnTo = executor.ExecuteScript(GetPageScript());
            var returnAddress = string.Empty;
            if (returnTo is string rr) returnAddress = rr;
            try
            {
                if (string.IsNullOrEmpty(_credential)) return false;
                if (!_credential.Contains(pipe)) return false;
                var secrets = _credential.Split(pipe);
                Uri uri = GetUri(destination);
                Driver.Navigate().GoToUrl(uri);
                if (!WaitForUserName()) return false;

                var loginjs = GetAuthScript(secrets[0], secrets[^1]);
                executor.ExecuteScript(loginjs);
                IsLoginRequested = false;
                isNavigationNeeded = true;
                WaitForHomePage();
                return true;
            }
            finally
            {
                if (isNavigationNeeded && !string.IsNullOrEmpty(returnAddress))
                {
                    Driver.Navigate().GoToUrl(returnAddress);
                }
            }
        }

        private bool WaitForUserName()
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10))
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

        private void WaitForHomePage()
        {
            try
            {
                var by = By.XPath("//a[@class='btn btn-lg btn-default portlet-buttons']");
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500),
                };
                wait.Until(d =>
                {
                    var uid = d.TryFindElement(by);
                    return uid != null;
                });
            }
            catch (Exception)
            {
                // intentionally left blank
            }
        }
        protected static string GetAuthScript(string userName, string password)
        {
            var scriptlet = new[]
            {
                $"var uid = '{userName}'",
                $"var pwd = '{password}'",
                "var userBox = document.getElementById('UserName');",
                "var wordBox = document.getElementById('Password');",
                "var chekBox = document.getElementById('TOSCheckBox');",
                "var signin = document.getElementById('btnSignIn');",
                "userBox.setAttribute('style', 'visibility: hidden');",
                "userBox.value = uid;",
                "wordBox.value = pwd;",
                "chekBox.click();",
                "while (signin.getAttribute('disabled') !== null ) { chekBox.click(); }",
                "signin.click();"
            };
            return string.Join(Environment.NewLine, scriptlet);
        }

        private static string GetPageScript()
        {
            var blocks = new[]
            {
                "if (!document) { return ''; }",
                "if (document.location == null) { return ''; }",
                "if (document.location.href == null) { return ''; }",
                "return String(document.location.href);",
            };
            return string.Join(Environment.NewLine, blocks);
        }
    }
}
