using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            if (!string.IsNullOrEmpty(dto.Address) && dto.Address.Contains("|"))
            {
                mapped.UpdateAddress(dto.Address.Split("|").ToList());
            }
            return mapped.ToCalculatedNames();
        }

        public static void UpdateAddress(this PersonAddress person, List<string> parts)
        {

            var ln = parts.Count - 1;
            var last = parts[ln].Trim();
            var pieces = last.Split(' ');
            person.Zip = pieces[pieces.Length - 1].Trim();
            person.Address3 = last;
            person.Address1 = parts[0];
            person.Address2 = string.Empty;
            if (ln > 1)
            {
                parts.RemoveAt(0); // remove first item
                if (parts.Count > 1) parts.RemoveAt(parts.Count - 1); // remove last, when applicable
                person.Address2 = string.Join(" ", parts);
            }
        }

        public static DateTime? GetCourtDate(this CaseItemDto dto)
        {
            if (dto == null) return null;
            if (string.IsNullOrWhiteSpace(dto.CourtDate)) return null;
            if (!DateTime.TryParse(dto.CourtDate, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var courtDt)) return null;
            return courtDt;
        }

        public static T ToInstance<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
                throw;
            }
        }

        public static string ToJsonString(this object obj)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonCast<T>(this object obj)
        {
            if (obj == null) return default;
            var json = obj.ToJsonString();
            return json.ToInstance<T>();
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
