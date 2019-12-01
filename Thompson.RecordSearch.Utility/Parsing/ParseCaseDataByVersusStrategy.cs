using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Parsing
{
    public class ParseCaseDataByVersusStrategy : ICaseDataParser
    {
        private static readonly string _searchKeyWord = @"vs.";

        public virtual string SearchFor => _searchKeyWord;

        public string CaseData { get; set; }

        public virtual bool CanParse()
        {
            if (string.IsNullOrEmpty(CaseData)) return false;
            if (!CaseData.ToLower().Contains(SearchFor)) return false;
            return true;
        }

        public virtual ParseCaseDataResponseDto Parse()
        {
            var response = new ParseCaseDataResponseDto { CaseData = CaseData };
            if (!CanParse()) return response;

            if (string.IsNullOrEmpty(CaseData)) return response;
            var fullName = CaseData.ToLower();
            if (!fullName.Contains(SearchFor)) return response;
            var findItIndex = fullName.IndexOf(SearchFor);
            if (findItIndex < 0) return response;
            response.Defendant = CaseData.Substring(findItIndex).Trim();
            response.Plantiff = CaseData.Substring(0, findItIndex).Trim();
            return response;            
        }
    }
}
