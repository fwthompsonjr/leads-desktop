﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class BexarBeginNavigation : BaseBexarSearchAction
    {
        public override int OrderId => 10;
        public override object Execute()
        {
            var destination = NavigationUri;

            if (Parameters == null || Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);
            Uri uri = GetUri(destination);

            Driver.Navigate().GoToUrl(uri);
            return true;
        }

        [ExcludeFromCodeCoverage]
        private static Uri GetUri(string destination)
        {
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
            const string web = "https://portal-txbexar.tylertech.cloud/Portal/Home/Dashboard/26";
            return web;
        }
    }
}