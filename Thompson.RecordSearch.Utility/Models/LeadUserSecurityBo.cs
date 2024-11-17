using System;

namespace Thompson.RecordSearch.Utility.Models
{
    public class LeadUserSecurityBo
    {
        public Guid Key { get; set; }
        public LeadUserModel User { get; set; }
        public string Reason { get; set; }
    }
}
