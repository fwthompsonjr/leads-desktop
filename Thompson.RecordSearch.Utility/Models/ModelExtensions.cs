using LegalLead.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Thompson.RecordSearch.Utility.Models
{
    public static class ModelExtensions
    {
        private static readonly string Defendant =
            ResourceTable.GetText(ResourceType.FindDefendant, ResourceKeyIndex.Defendant);
        const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
        
        
        public static List<HLinkDataRow> ConvertToDataRow(this CaseRowData source)
        {
            if (source == null) return null;
            var dest = new List<HLinkDataRow>();
            var defentdants = source.CaseDataAddresses
                .Where(x => x.Role.Equals(Defendant, ccic))
                .ToList();
            foreach (var person in defentdants)
            {
                var row = new HLinkDataRow
                {
                    WebsiteId = source.RowId,
                    Address = GetAddress(person),
                    Case = person.Case,
                    CaseStyle = source.Style,
                    CaseType = source.TypeDesc,
                    Court = source.Court,
                    DateFiled = source.FileDate,
                    Defendant = GetPerson(person),
                    Data = person.Party
                };
                dest.Add(row);
            }
            return dest;
        }

        public static string GetPerson(this CaseDataAddress source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var pipe = '|';
            var pipeString = "|";
            const string noMatch = "No Person Associated";
            var person = source.Party;
            if (string.IsNullOrEmpty(person)) { person = noMatch; }
            if (person.EndsWith(pipeString, ccic))
            {
                person = person.Substring(0, person.Length - 1);
            }
            var pieces = person.Split(pipe)
                .ToList().FindAll(s => !string.IsNullOrEmpty(s));
            if (!pieces.Any())
            {
                return person;
            }
            return pieces[0].Trim();
        }

        public static string GetAddress(this CaseDataAddress source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var pipe = '|';
            var pipeString = "|";
            const string noMatch = "No Match Found|Not Matched 00000";
            var address = source.Party;
            if (string.IsNullOrEmpty(address)) { 
                return noMatch; 
            }
            address = address.Trim();
            if (address.EndsWith(pipeString, ccic))
            {
                address = address.Substring(0, address.Length - 1);
            }
            var pieces = address.Split(pipe)
                .ToList().FindAll(s => !string.IsNullOrEmpty(s));
            if (!pieces.Any())
            {
                return noMatch;
            }
            pieces.ForEach(x => x = x.Trim());
            address = string.Empty;
            // get the person part of this address
            for (int i = 0; i < pieces.Count; i++)
            {
                if (i == 0) continue;
                var piece = pieces[i].Trim();
                if (string.IsNullOrEmpty(address))
                {
                    address = piece;
                }
                else
                {
                    address = (address + pipeString + piece);
                }
            }
            return address;

        }

        public static string ToHtml(this List<CaseRowData> source)
        {
            var template = new StringBuilder();
            // 
            string header = "<table>";
            template.Append(header);

            template.AppendLine("<thead>");
            template.AppendLine("<tr>");
            template.AppendLine("<th> Case </th>");
            template.AppendLine("<th> Style </th>");
            template.AppendLine("<th> DateFiled </th>");
            template.AppendLine("<th> Court </th>");
            template.AppendLine("<th> CaseType </th>");
            template.AppendLine("<th> Status </th>");
            template.AppendLine("</tr>");

            template.AppendLine("</thead>");

            template.AppendLine("<tbody> ");

            if (source != null)
            {
                source.ForEach(s => 
                { 
                    template.AppendLine(s.ToHtml()); 
                });
            }

            template.AppendLine("</tbody> ");
            template.AppendLine("</table> ");
            return template.ToString();
        }

        public static string ToHtml(this CaseRowData source)
        {
            var template = new StringBuilder();
            template.AppendLine("<tr>");
            template.AppendLine("<td>[Case]</td>");
            template.AppendLine("<td>[Style]</td>");
            template.AppendLine("<td>[DateFiled]</td>");
            template.AppendLine("<td>[Court]</td>");
            template.AppendLine("<td>[CaseType]</td>");
            template.AppendLine("<td>[Status]</td>");
            template.AppendLine("</tr>");
            if (source == null) return template.ToString();
            template.Replace("[RowIndex]", source.RowId.ToString());
            template.Replace("[Case]", System.Net.WebUtility.HtmlEncode(source.Case));
            template.Replace("[Style]", System.Net.WebUtility.HtmlEncode(source.Style));
            template.Replace("[DateFiled]", System.Net.WebUtility.HtmlEncode(source.FileDate));
            template.Replace("[Court]", System.Net.WebUtility.HtmlEncode(source.Court));
            template.Replace("[CaseType]", System.Net.WebUtility.HtmlEncode(source.TypeDesc));
            template.Replace("[Status]", System.Net.WebUtility.HtmlEncode(source.Status));
            return template.ToString();
        }

    }
}
