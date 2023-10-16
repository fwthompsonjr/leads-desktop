using System.Collections.Generic;

namespace Thompson.RecordSearch.Utility.Models
{
    public class CaseDataAddress
    {
        public string Case { get; set; }
        public string Role { get; set; }
        public string Party { get; set; }
        public string Attorney { get; set; }
    }

    public class CaseRowData
    {
        public int RowId { get; set; }
        public string Case { get; set; }
        public string Court { get; set; }
        public string FileDate { get; set; }
        public string Status { get; set; }
        public string TypeDesc { get; set; }
        public string Subtype { get; set; }
        public string Style { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public IList<CaseDataAddress> CaseDataAddresses { get; set; }
    }


}
