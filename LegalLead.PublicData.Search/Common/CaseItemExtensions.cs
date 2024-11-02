using System;
using System.Linq;
using System.Text;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Common
{
    public static class CaseItemExtensions
    {

        public static void SetPartyNameFromCaseStyle(this CaseItemDto dto, bool overwrite = false)
        {
            const string versus = "VS. ,VS ,vs ,vs. ,vs.,v. ,EX PARTE: ,IN RE: ";
            const char comma = ',';
            if (!overwrite && !string.IsNullOrWhiteSpace(dto.PartyName)) return;
            var arr = versus.Split(comma);
            var caseStyle = ClearWhiteSpace(dto.CaseStyle);
            if (string.IsNullOrEmpty(caseStyle)) return;
            ItemDto match = FindPartyInCaseStyle(arr, caseStyle);
            if (match == null) return;
            var find = match.Find;
            var indx = match.Id;
            var name = FindPartyName(indx, indx + find.Length, caseStyle);
            if (string.IsNullOrEmpty(name)) return;
            dto.PartyName = name;
        }

        private static ItemDto FindPartyInCaseStyle(string[] arr, string caseStyle)
        {
            ItemDto match = null;
            for (int i = 0; i < 2; i++)
            {
                var indexes = arr.Select(a =>
                {
                    if (i == 0) return new ItemDto { Id = caseStyle.IndexOf(a), Find = a };
                    else return new ItemDto { Id = caseStyle.IndexOf(a, StringComparison.OrdinalIgnoreCase), Find = a };
                }
                );
                match = indexes.FirstOrDefault(x => x.Id >= 0);
                if (match != null) break;
            }

            return match;
        }
        private static string FindPartyName(int indx, int startIndex, string caseStyle)
        {
            var oic = StringComparison.OrdinalIgnoreCase;
            const string etal = "ET AL";
            const char comma = ',';
            const char pipe = '|';
            const char space = ' ';
            const string suffixes = ", Jr.|, Sr.|, I|, II|, III|, IV|, V";
            if (indx < 0) return string.Empty;
            if (startIndex > caseStyle.Length - 1) return string.Empty;
            var name = caseStyle[(startIndex)..].Trim();
            if (name.EndsWith(etal)) name = name[..^etal.Length].Trim();
            var suffix = string.Empty;
            var arr = suffixes.Split(pipe);
            for (int i = 0; i < arr.Length; i++)
            {
                var find = arr[i];
                if (!name.EndsWith(find, oic)) continue;
                suffix = find.Replace(comma.ToString(), string.Empty);
                name = name[..^find.Length].Trim();
            }
            if (name.Contains(space))
            {
                var names = name.Split(space).ToList();
                if (names.Count == 2)
                {
                    name = $"{names[1]}{suffix}, {names[0]}";
                }
                if (names.Count > 2)
                {
                    var ending = string.Join(" ", names.GetRange(1, names.Count - 1));
                    name = $"{ending}{suffix}, {names[0]}";
                }
            }
            return name;
        }
        private static string ClearWhiteSpace(string str)
        {
            const char spc = ' ';
            const string space = " ";
            const string doublespace = "  ";
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            var temp = str.Trim();
            var collection = temp.ToCharArray();
            var builder = new StringBuilder();
            foreach (var c in collection)
            {
                if (string.IsNullOrWhiteSpace(c.ToString())) builder.Append(spc);
                else builder.Append(c);
            }
            builder.Replace(doublespace, space);
            var final = builder.ToString();
            while (final.Contains(doublespace)) { final = final.Replace(doublespace, space); }
            return final.Trim();
        }

        private sealed class ItemDto
        {
            public string Find { get; set; }
            public int Id { get; set; }
        }
    }
}
