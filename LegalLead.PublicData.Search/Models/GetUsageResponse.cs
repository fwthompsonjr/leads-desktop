using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.PublicData.Search.Models
{
    public class GetUsageResponse : GetUsageRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}
