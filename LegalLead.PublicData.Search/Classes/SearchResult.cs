using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.PublicData.Search.Classes
{
    public class SearchResult
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string Website { get; set; }

        public string ResultFileName { get; set; }

        public string Search { get; set; }

        public string SearchDate { get; set; }

        public bool IsCompleted { get; set; }
        public int Id { get; internal set; }
    }
}
