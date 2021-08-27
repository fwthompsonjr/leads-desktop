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
        public List<HarrisCriminalDto> Data { get; set; }
        public List<HarrisCriminalBo> BusinessData { get; set; }
    }
}
