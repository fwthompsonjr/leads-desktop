using System;
using System.Collections.Generic;

namespace Harris.Criminal.Db.Downloads
{
    public class HarrisCountyListDto
    {
        public string Name { get; set; }
        public DateTime FileDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime MaxFilingDate { get; set; }
        public DateTime MinFilingDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage",
            "CA2227:Collection properties should be read only",
            Justification = "Data is written during startup process")]
        public List<HarrisCriminalDto> Data { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage",
            "CA2227:Collection properties should be read only",
            Justification = "Data is written during startup process")]
        public List<HarrisCriminalBo> BusinessData { get; set; }
    }
}
