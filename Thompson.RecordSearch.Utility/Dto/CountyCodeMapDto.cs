using System.Collections.Generic;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class CountyCodeMapDto
    {
        public string Web { get; set; } = string.Empty;
        public CountyCodeLandingDto Landings { get; set; } = new CountyCodeLandingDto();
        public IEnumerable<CountyCodeDto> Counties { get; set; } = Enumerable.Empty<CountyCodeDto>();
    }

    public class CountyCodeLandingDto
    {
        public string Login { get; set; }
        public string County { get; set; }
    }
}
