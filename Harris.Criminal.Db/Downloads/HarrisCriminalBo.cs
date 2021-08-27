using System;
using System.Collections.Generic;
using System.Linq;

namespace Harris.Criminal.Db.Downloads
{
    public class HarrisCriminalBo : HarrisCriminalDto
    {
        private static string dteFmt = "yyyyMMdd";
        private static DateTime MinDate = new DateTime(1900, 1, 1);
        public DateTime DateFiled => FilingDate.ToExactDate(dteFmt, MinDate);


        public static List<HarrisCriminalBo> Map(List<HarrisCriminalDto> data)
        {
            if (data == null) return default;
            if (!data.Any()) return default;
            var result = new List<HarrisCriminalBo>();
            var fields = FieldNames;
            foreach (var item in data)
            {
                var bo = new HarrisCriminalBo();
                fields.ForEach(f => { bo[f] = item[f]; });
                result.Add(bo);
            }
            return result;
        }
    }
}
