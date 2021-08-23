using System;
using System.Linq;
using Thompson.RecordSearch.Utility;
using Thompson.RecordSearch.Utility.Classes;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search
{
    internal class WebFetchingProvider
    {
        internal static IWebInteractive GetInteractive(
            WebNavigationParameter siteData,
            DateTime startDate,
            DateTime endingDate)
        {
            switch (siteData.Id)
            {
                case 10:
                    return new TarrantWebInteractive(siteData, startDate, endingDate);
                case 20:
                    return new CollinWebInteractive(siteData, startDate, endingDate);
                case 30:
                    return new HarrisCivilInteractive(siteData, startDate, endingDate);
                default:
                    var districtKey = Program.DentonCustomKeys.FirstOrDefault(
                        x => x.Name.Equals(CommonKeyIndexes.DistrictSearchType, // "DistrictSearchType", 
                        StringComparison.CurrentCulture));
                    var siteDistrictKey = siteData.Keys.FirstOrDefault(
                        x => x.Name.Equals(CommonKeyIndexes.DistrictSearchType,
                        StringComparison.CurrentCulture));
                    if (districtKey == null)
                    {
                        if (siteDistrictKey != null)
                        {
                            siteData.Keys.Remove(siteDistrictKey);
                        }
                    }
                    else
                    {
                        if (siteDistrictKey == null)
                        {
                            siteData.Keys.Add(districtKey);
                        }
                    }
                    return new WebInteractive(siteData, startDate, endingDate);
            }
        }
    }
}
