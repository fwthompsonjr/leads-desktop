using System;
using System.Collections.Generic;
using System.Linq;

namespace Harris.Criminal.Db.Downloads
{
    public class HarrisCriminalBo : HarrisCriminalDto
    {
        private const string dteFmt = "yyyyMMdd";
        private static readonly DateTime MinDate = new DateTime(1900, 1, 1);
        private string _literalCaseStatus;
        public DateTime DateFiled => FilingDate.ToExactDate(dteFmt, MinDate);

        public string LiteralCaseStatus
        {
            get
            {
                if (_literalCaseStatus != null)
                {
                    return _literalCaseStatus;
                }
                const StringComparison oic = StringComparison.OrdinalIgnoreCase;
                const string fieldName = "cst";
                var tableName = $"HCC.Tables.{fieldName}";
                int fieldId = AliasNames.IndexOf(fieldName);
                var actualFieldValue = this[fieldId]?.Trim();
                if (string.IsNullOrEmpty(actualFieldValue))
                {
                    _literalCaseStatus = string.Empty;
                    return _literalCaseStatus;
                }
                var dataTable = Startup.References.DataList.FirstOrDefault(f => f.Name.Equals(tableName, oic));
                var dataItem = dataTable?.Data.FirstOrDefault(d => d.Code.Equals(actualFieldValue, oic));
                _literalCaseStatus = dataItem?.Literal ?? string.Empty;
                return _literalCaseStatus;
            }
        }

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
