using System;

namespace Thompson.RecordSearch.Utility.Dto
{
    public class HarrisCriminalStyleBo : HarrisCriminalStyleDto
    {
        public DateTime? DateFiled => GetDate(FileDate);
        public string StatusDefendant => GetString(Status, "Defendant:");
        public string StatusDisposition => GetString(Status, "Disposition:");

        public static HarrisCriminalStyleBo MapFrom(HarrisCriminalStyleDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            var bo = new HarrisCriminalStyleBo();
            FieldNames.ForEach(f => { bo[f] = dto[f]; });
            return bo;
        }

        private static DateTime? GetDate(string fileDate)
        {
            if (string.IsNullOrEmpty(fileDate))
            {
                return default;
            }
            if (DateTime.TryParse(fileDate, out DateTime date))
            {
                return date;
            }
            return default;
        }
        private static string GetString(string status, string search)
        {
            if (string.IsNullOrEmpty(status) | string.IsNullOrEmpty(search))
            {
                return default;
            }
            var data = search.Split(Environment.NewLine.ToCharArray());
            foreach (var line in data)
            {
                if (!line.StartsWith(search, StringComparison.OrdinalIgnoreCase))
                {
                    var result = line.Replace(search, "").Trim();
                    return result;
                }
            }
            return default;
        }
    }
}
