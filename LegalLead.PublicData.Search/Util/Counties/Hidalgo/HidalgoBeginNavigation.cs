using System;
using System.Diagnostics.CodeAnalysis;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class HidalgoBeginNavigation : BaseHidalgoSearchAction
    {
        public override int OrderId => 10;
        public override object Execute()
        {
            var destination = NavigationUri();

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
    }
}