// ParseCaseOrderForForeclosure

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Parsing
{

    public class ParseCaseOrderForForeclosure : ICaseDataParser
    {
        private static readonly string _searchKeyWord = @"in re: order for foreclosure concerning ";

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
            const string petitioner = "Petitioner:";

            var response = new ParseCaseDataResponseDto { CaseData = CaseData };
            if (!CanParse()) return response;

            if (string.IsNullOrEmpty(CaseData)) return response;
            var fullName = CaseData.ToLower();
            if (!fullName.StartsWith(SearchFor)) return response;

            var findItIndex = fullName.IndexOf(SearchFor);
            if (findItIndex < 0) return response;
            fullName = CaseData.Substring(SearchFor.Length);
            var splitIndex = fullName.IndexOf(petitioner);
            fullName = fullName.Substring(0, splitIndex + petitioner.Length);
            fullName = CaseData.Substring(SearchFor.Length).Substring(fullName.Length).Trim();
            splitIndex = fullName.LastIndexOf(':');
            if(splitIndex > 0)
            {
                fullName = fullName.Substring(0, splitIndex).Trim();
                splitIndex = fullName.LastIndexOf(' ');
                if(splitIndex > 0)
                {
                    fullName = fullName.Substring(0, splitIndex).Trim();
                }
            }
            response.Plantiff = fullName;
            fullName = CaseData.Substring(findItIndex).Trim();
            splitIndex = fullName.LastIndexOf(':');
            response.Defendant = fullName.Substring(splitIndex).Replace(":","").Trim();
            return response;
        }
    }
}
