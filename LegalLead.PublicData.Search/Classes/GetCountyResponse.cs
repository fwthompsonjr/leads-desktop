using System;

namespace LegalLead.PublicData.Search.Classes
{
    public class GetCountyResponse : IEquatable<GetCountyResponse>
    {
        public string Id { get; set; }
        public int? RwId { get; set; }
        public int? CountyId { get; set; }
        public string LeadUserId { get; set; }
        public string CountyName { get; set; }
        public string UserName { get; set; }
        public string Credential { get; set; }
        public int? MonthlyUsage { get; set; }
        public DateTime? CreateDate { get; set; }

        // Override the Equals method
        public override bool Equals(object obj)
        {
            // Check if the object is null or not of the same type
            if (obj is not GetCountyResponse compare) return false;
            if (!RwId.GetValueOrDefault().Equals(compare.RwId.GetValueOrDefault())) return false;
            var src = MonthlyUsage;
            var dest = compare.MonthlyUsage;
            if (!src.HasValue && !dest.HasValue) return true;
            // Compare the properties
            return src.GetValueOrDefault().Equals(dest.GetValueOrDefault());
        }

        public bool Equals(GetCountyResponse other)
        {
            return Equals((object)other);
        }

        // Override the GetHashCode method
        public override int GetHashCode()
        {
            // Combine the hash codes of the properties
            return HashCode.Combine(Id, RwId.GetValueOrDefault(), CountyId.GetValueOrDefault());
        }
    }
}
