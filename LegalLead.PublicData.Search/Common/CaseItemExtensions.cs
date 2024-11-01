using System.Linq;
using System.Text;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Common
{
    public static class CaseItemExtensions
    {

        public static void SetPartyNameFromCaseStyle(this CaseItemDto dto, bool overwrite = false)
        {
            const string find = "VS. ";
            const string etal = "ET AL";
            const char space = ' ';
            if (!overwrite && !string.IsNullOrWhiteSpace(dto.PartyName)) return;
            var caseStyle = ClearWhiteSpace(dto.CaseStyle);
            if (string.IsNullOrEmpty(caseStyle)) return;
            if (!caseStyle.Contains(find)) return;
            var indx = caseStyle.IndexOf(find);
            var name = indx == -1 ? string.Empty : caseStyle[(indx + find.Length)..];
            if (name.EndsWith(etal))
            {
                name = name[..^etal.Length].Trim();
            }
            if (name.Contains(space))
            {
                var names = name.Split(space).ToList();
                if (names.Count == 2)
                {
                    name = $"{names[1]}, {names[0]}";
                }
                if (names.Count > 2)
                {
                    var ending = string.Join(" ", names.GetRange(1, names.Count - 1));
                    name = $"{ending}, {names[0]}";
                }
            }
            dto.PartyName = name;
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
    }
}
