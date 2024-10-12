using StructureMap.Building;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    using Rx = Properties.Resources;
    public class TravisBeginNavigation : BaseTravisSearchAction
    {
        public override int OrderId => 10;
        public override object Execute()
        {   
            if (Parameters == null || Driver == null)
                throw new NullReferenceException(Rx.ERR_DRIVER_UNAVAILABLE);

            var destination = GetNavigationUri(Parameters.CourtType);
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

        private static string GetNavigationUri(string courtType = "Justice")
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            var find = courtType.ToUpper(culture);
            var obj = TravisScriptHelper.NavigationSetting;
            var address = obj.JusticeWebsite;
            if (find.Equals("COUNTY", StringComparison.OrdinalIgnoreCase)) address = obj.CountyWebsite;
            if (find.Equals("DISTRICT", StringComparison.OrdinalIgnoreCase)) address = obj.DistrictWebsite;
            return address;
        }
    }
}