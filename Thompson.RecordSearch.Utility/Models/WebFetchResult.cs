using System.Collections.Generic;

namespace Thompson.RecordSearch.Utility.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public class WebFetchResult
    {
        public int WebsiteId { get; set; }
        public List<PersonAddress> PeopleList { get; set; }
        public string CaseList { get; set; }
        public string Result { get; set; }
    }
}
