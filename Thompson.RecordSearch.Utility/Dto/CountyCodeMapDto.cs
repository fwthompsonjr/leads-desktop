using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class CountyCodeMapDto
    {
        public string Web { get; set; } = string.Empty;
        public CountyCodeLandingDto Landings { get; set; } = new CountyCodeLandingDto();

        [JsonProperty("apilandings")]
        public CountyCodeLandingDto ApiLandings { get; set; } = new CountyCodeLandingDto();
        public IEnumerable<CountyCodeDto> Counties { get; set; } = Enumerable.Empty<CountyCodeDto>();
    }

    public class CountyCodeLandingDto
    {
        public string Login { get; set; } = string.Empty;
        public string County { get; set; } = string.Empty;
        public string Change { get; set; } = string.Empty;
        public string Indexes { get; set; } = string.Empty;
        public string Register { get; set; } = string.Empty;
    }
}
