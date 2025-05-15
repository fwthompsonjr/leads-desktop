using LegalLead.PublicData.Search.Common;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarAuthenicateActor : BaseBexarSearchAction
    {
        public override int OrderId => 12;
        public IJavaScriptExecutor ExternalExecutor { get; set; } = null;
        public override object Execute()
        {
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            var executor = ExternalExecutor ?? Driver.GetJsExecutor();
            var destination = NavigationUri(executor);
            if (string.IsNullOrEmpty(destination)) return null;
            _ = GetUri(destination);
            return destination;
        }

        [ExcludeFromCodeCoverage]
        private static Uri GetUri(string destination)
        {
            if (!Uri.TryCreate(destination, UriKind.Absolute, out var uri))
                throw new ArgumentException(Rx.ERR_URI_MISSING);
            return uri;
        }

        private static string navigationUri = null;
        private static string NavigationUri(IJavaScriptExecutor executor)
        {
            if (!string.IsNullOrEmpty(navigationUri)) return navigationUri;
            navigationUri = GetNavigationUri(executor);
            return navigationUri;
        }

        private static string GetNavigationUri(IJavaScriptExecutor executor)
        {
            if (executor == null) return string.Empty;
            string web = string.Join(Environment.NewLine, parameters);
            var address = executor.ExecuteScript(web);
            if (address is not string rsp) return string.Empty;
            return rsp;
        }

        private readonly static List<string> parameters = new()
        {
            "var menu = document.getElementById('dropdownMenu1');",
            "var div = menu.closest('div');",
            "var uls = Array.prototype.slice.call( div.getElementsByTagName('ul'), 0 );",
            "if (null != uls && uls.length > 0) {",
            "	litem = Array.prototype.slice.call( uls[0].getElementsByTagName('li'), 0 )",
            "	.find(x => x.innerText.toLowerCase().indexOf('sign in') >= 0);",
            "	if (litem != null) {",
            "		sfx = litem.children[0].getAttribute('href');",
            "		if (null != sfx) {",
            "			pfx = ''.concat(document.location.protocol, '//', document.location.hostname)",
            "			if (pfx.endsWith('/')) { pfx = pfx.substr(0, pfx.length - 1) }",
            "		}",
            "		address = ''.concat(pfx, sfx);",
            "		return address;",
            "	}",
            "}"
        };
    }
}