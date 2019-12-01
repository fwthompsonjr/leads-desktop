// ParseProtectiveOrderCaseType
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Parsing
{
    public class ParseProtectiveOrderCaseType : ICaseDataParser
    {
        // Name Change of:
        private static readonly string _searchKeyWord = @"protective order| ";

        public virtual string SearchFor => _searchKeyWord;

        public string CaseData { get; set; }

        public virtual bool CanParse()
        {
            if (string.IsNullOrEmpty(CaseData)) return false;
            if (!CaseData.ToLower().StartsWith(SearchFor)) return false;
            var lowered = CaseData.ToLower();
            var firstAnd = lowered.Substring(SearchFor.Length).IndexOf(" and ");
            if (firstAnd < 0) return false;
            return true;
        }
        public virtual ParseCaseDataResponseDto Parse()
        {
            const string and = " and ";
            var response = new ParseCaseDataResponseDto { CaseData = CaseData };
            if (!CanParse()) return response;

            if (string.IsNullOrEmpty(CaseData)) return response;
            var fullName = CaseData.ToLower();
            if (!fullName.StartsWith(SearchFor)) return response;

            var findItIndex = fullName.IndexOf(SearchFor);
            if (findItIndex < 0) return response;
            //response.Defendant = CaseData.Substring(findItIndex).Trim();
            fullName = CaseData.Substring(SearchFor.Length).Trim();
            var splitIndex = fullName.IndexOf(and);
            if (splitIndex < 0)
            {
                response.Plantiff = fullName.Trim();
                return response;
            }
            response.Plantiff = fullName.Substring(fullName.IndexOf(and)).Replace(and, string.Empty).Trim(); ;
            return response;
        }
    }
}
