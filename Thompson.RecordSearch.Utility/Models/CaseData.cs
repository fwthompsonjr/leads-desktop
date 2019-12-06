using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Thompson.RecordSearch.Utility.Models
{
    public class CaseData
    {

        private const string FieldNames = @"Case Number,Case Name,Filed/Location/Judical Officer,Type/Status";
        private string _fieldNames;
        private List<string> _fieldList;

        public string CaseNumber { get; set; }

        public string CaseName { get; set; }

        public string FilingInfo { get; set; }

        public string TypeStatus { get; set; }

        protected string LoweredFieldNames
        {
            get
            {
                return _fieldNames ?? (_fieldNames = FieldNames.ToLower(CultureInfo.CurrentCulture));
            }
        }

        public List<string> FieldList
        {
            get
            {
                return _fieldList ?? (
                  _fieldList = LoweredFieldNames.Split(',').ToList());
            }
        }

        public string this[string indexName]
        {
            get
            {
                if (string.IsNullOrEmpty(indexName)) return string.Empty;
                var keyName = indexName.ToLower(CultureInfo.CurrentCulture);
                if (!FieldList.Contains(keyName)) return string.Empty;

                switch (keyName)
                {
                    case "case number":
                        return CaseNumber;
                    case "case name":
                        return CaseName;
                    case "filed/location/judical officer":
                        return FilingInfo;
                    case "type/status":
                        return TypeStatus;
                    default:
                        return string.Empty;
                }
            }
            set
            {

                if (string.IsNullOrEmpty(indexName)) return;
                var keyName = indexName.ToLower(CultureInfo.CurrentCulture);
                if (!FieldList.Contains(keyName)) return;

                switch (keyName)
                {
                    case "case number":
                        CaseNumber = value;
                        return;
                    case "case name":
                        CaseName = value;
                        return;
                    case "filed/location/judical officer":
                        FilingInfo = value;
                        return;
                    case "type/status":
                        TypeStatus = value;
                        return;
                    default:
                        return;
                }
            }
        }

        public string this[int index]
        {
            get
            {
                if (index < 0) return string.Empty;
                if (index > FieldList.Count - 1) return string.Empty;
                return this[FieldList[index]];
            }
            set
            {

                if (index < 0) return;
                if (index > FieldList.Count - 1) return;
                this[FieldList[index]] = value;
            }
        }
    }
}
