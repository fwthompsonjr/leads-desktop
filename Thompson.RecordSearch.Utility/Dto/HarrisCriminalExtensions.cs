using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Dto
{
    public static class HarrisCriminalExtensions
    {
        public static void Append(this List<HarrisCriminalStyleDto> list, IEnumerable<HarrisCriminalStyleDto> source)
        {
            if (list == null) return;
            if (source == null) return;
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            foreach (var item in source)
            {
                var found = list.FirstOrDefault(f => f.CaseNumber.Equals(item.CaseNumber, oic) &
                    f.Court.Equals(item.Court, oic) &
                    f.FileDate.Equals(item.FileDate, oic));
                if (found == null) { list.Add(item); }
            }
        }
    }
}
