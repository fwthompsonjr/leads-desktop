using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Util
{
    public static class AlternateCourtLookupService
    {
        public static string GetAddress(int countyId, string court)
        {
            const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
            var websiteId = countyId.ToString("0", formatNumber);
            var courtReference = collection.Find(c => c.Id.Equals(websiteId, ccic));
            if (courtReference == null) return string.Empty;
            var courtLocation = courtReference.Courts
                .FirstOrDefault(a =>
                    a.Name.Equals(court, ccic)
                    | a.FullName.Equals(court, ccic));
            if (courtLocation != null) return courtLocation.Address;
            var blankLocation = courtReference.Courts
                .FirstOrDefault(a => a.Name.Equals("default", ccic));
            return blankLocation?.Address ?? string.Empty;
        }

        private static List<CourtLocation> UpdateEmptyValues(IList<CourtLocation> courts)
        {
            courts.ToList().ForEach(c =>
            {
                c.Courts.Where(a => string.IsNullOrEmpty(a.FullName))
                .ToList()
                .ForEach(b => b.FullName = string.Empty);
                c.Courts
                    .Where(a => string.IsNullOrEmpty(a.Name))
                    .ToList()
                    .ForEach(b => b.Name = string.Empty);
            });
            return courts.ToList();
        }

        private static readonly NumberFormatInfo formatNumber = new();
        private static readonly List<CourtLocation> collection =
            UpdateEmptyValues(SearchSettingDto.GetCourtLookupList.CourtLocations);
    }
}