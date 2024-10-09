using System.Collections.Generic;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class CountyCodeMapDto
    {
        public string Web { get; set; } = string.Empty;
        public IEnumerable<CountyCodeDto> Counties { get; set; } = Enumerable.Empty<CountyCodeDto>();
    }
}
