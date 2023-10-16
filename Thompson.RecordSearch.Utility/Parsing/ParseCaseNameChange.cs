using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Parsing
{
    using CCulture = System.Globalization.CultureInfo;
    public class ParseCaseNameChange : ICaseDataParser
    {
        const System.StringComparison comparison =
            System.StringComparison.CurrentCultureIgnoreCase;
        // Name Change of:
        private const string _searchKeyWord = @"name change of: ";

        public virtual string SearchFor => _searchKeyWord;

        public string CaseData { get; set; }

        public virtual bool CanParse()
        {
            if (string.IsNullOrEmpty(CaseData))
            {
                return false;
            }

            if (!CaseData.ToLower(CCulture.CurrentCulture)
                .StartsWith(SearchFor, comparison))
            {
                return false;
            }

            return true;
        }
        public virtual ParseCaseDataResponseDto Parse()
        {
            var response = new ParseCaseDataResponseDto { CaseData = CaseData };
            if (!CanParse())
            {
                return response;
            }

            if (string.IsNullOrEmpty(CaseData))
            {
                return response;
            }

            var fullName = CaseData.ToLower(CCulture.CurrentCulture);
            if (!fullName.StartsWith(SearchFor, comparison))
            {
                return response;
            }

            var findItIndex = fullName.IndexOf(SearchFor, comparison);
            if (findItIndex < 0)
            {
                return response;
            }
            //response.Defendant = CaseData.Substring(findItIndex).Trim();
            response.Plantiff =
                CaseData.Substring(SearchFor.Length).Trim();
            return response;
        }
    }
}
