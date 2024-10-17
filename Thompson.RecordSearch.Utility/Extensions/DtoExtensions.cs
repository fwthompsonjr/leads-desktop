using System.Collections.Generic;
using System.Web;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Extensions
{
    public static class DtoExtensions
    {
        public static PersonAddress FromDto(this CaseItemDto dto)
        {
            if (dto == null) return null;
            var mapped = new PersonAddress
            {
                Court = dto.Court,
                CaseNumber = dto.CaseNumber,
                CaseStyle = DecodeString(dto.CaseStyle),
                CaseType = dto.CaseType,
                DateFiled = dto.FileDate,
                Status = dto.CaseStatus,
                Name = DecodeString(dto.PartyName),
                Plantiff = DecodeString(dto.Plaintiff),
                Zip = "00000",
                Address1 = "000 No Street",
                Address3 = "Not, NA"
            };
            return mapped.ToCalculatedNames();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Qube", 
            "S1854:Unused assignments should be removed", 
            Justification = "False positive. Variable assignment is needed to update item in list")]
        public static void ReMapNameFromCaseStyle(this List<PersonAddress> people)
        {
            const string find = " vs.";
            if (people == null) return;
            var subset = people.FindAll(x => string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrEmpty(x.CaseStyle));
            if (subset.Count == 0) return;
            subset.ForEach(s =>
            {
                var caseStyle = s.CaseStyle; var vsIndex = caseStyle.IndexOf(find, System.StringComparison.OrdinalIgnoreCase);
                var response = string.Empty;
                if (vsIndex > 0)
                {
                    response = caseStyle.Substring(vsIndex + find.Length).Trim();
                    response = response.Replace(",", ", ");
                    response = response.Replace("  ", " ");
                }
                s.Name = response;
                s = s.ToCalculatedNames();
            });
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", 
            "CA1031:Do not catch general exception types", 
            Justification = "Excluding this method as it is a private static with behavior tested and covered")]
        private static string DecodeString(string data)
        {
            try
            {
                return HttpUtility.HtmlDecode(data);
            }
            catch
            {
                return data;
            }
        }
    }
}
