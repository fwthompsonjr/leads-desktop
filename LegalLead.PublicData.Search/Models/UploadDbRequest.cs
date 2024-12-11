using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Models
{
    public class UploadDbRequest
    {
        public string Id { get; set; }
        public List<QueryDbResponse> Contents { get; set; }
    }
}
