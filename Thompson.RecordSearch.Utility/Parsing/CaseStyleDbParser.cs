using System;
using System.Globalization;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Parsing
{
    public class CaseStyleDbParser
    {
        private const string _searchKeyWord = @" vs.";
        private const StringComparison Oic = StringComparison.OrdinalIgnoreCase;
        private enum DataExtractType
        {
            CaseData,
            Defendant,
            Plantiff
        }

        /// <summary>
        /// Gets the specific keyword used to parse plantiff from defendant.
        /// </summary>
        /// <value>
        /// The search keyword used by this parsing strategy.
        /// </value>
        public virtual string SearchFor => _searchKeyWord;

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data { get; set; }


        public virtual bool CanParse()
        {
            if (string.IsNullOrEmpty(Data)) return false;
            if (!Data.ToLower(CultureInfo.CurrentCulture).Contains(SearchFor)) return false;
            return true;
        }

        public virtual ParseCaseStyleDbDto Parse()
        {
            var response = new ParseCaseStyleDbDto { Data = Data };
            if (!CanParse()) return response;

            if (string.IsNullOrEmpty(Data)) return response;
            response.CaseData = ExtractField(DataExtractType.CaseData, Data);
            response.Defendant = ExtractField(DataExtractType.Defendant, response.CaseData);
            response.Plantiff = ExtractField(DataExtractType.Plantiff, response.CaseData);
            return response;
        }

        private static string ExtractField(DataExtractType extractType, string data)
        {
            var response = string.Empty;
            if (string.IsNullOrEmpty(data)) return response;
            switch (extractType)
            {
                case DataExtractType.CaseData:
                    var a = data.IndexOf("(", Oic);
                    if (a < 0) return data.Trim();
                    response = data.Substring(0, a).Trim();
                    return response;
                case DataExtractType.Defendant:
                case DataExtractType.Plantiff:
                    var b = data.IndexOf(_searchKeyWord, Oic);
                    if (b < 0) return data;
                    var info = data.Split(_searchKeyWord.ToCharArray());
                    var index = extractType == DataExtractType.Plantiff ? 0 : 1;
                    response = info[index].Trim();
                    return response;
                default:
                    return response;
            }
        }
    }
}
