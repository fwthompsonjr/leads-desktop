using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.PublicData.Search.Models
{
    public class GetMonthlyLimitRequest
    {
        public string LeadId { get; set; } = string.Empty;
        public int CountyId { get; set; }
        public bool GetAllCounties { get; set; }
    }
}
