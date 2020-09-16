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
                    Address = person.Party,
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

            // get the person part of this address
            for (int i = 0; i < pieces.Count; i++)
            {
                if(i == 0)
                {
                    person = pieces[i].Trim();
                    continue;
                }
                if (IsStreetAddress(pieces[i]))
                {
                    break;
                }
                person += ", " + pieces[i].Trim();
            }
            return person;
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
            bool isAddressMatched = false;
            // get the person part of this address
            for (int i = 0; i < pieces.Count; i++)
            {
                if (!isAddressMatched)
                {
                    if (IsStreetAddress(pieces[i]))
                    {
                        isAddressMatched = true;
                        address = pieces[i].Trim();
                        continue;
                    }
                }
                address += pipeString + pieces[i].Trim();
            }
            return address;

        }
        public static bool IsStreetAddress(string input)
        {
            string pattern = @"\d+(\s|-)?\w*$";
            RegexOptions options = RegexOptions.Singleline;

            foreach (Match m in Regex.Matches(input, pattern, options))
            {
                if (m.Index <= 1) return true;
            }
            return false;
        }
    }
}
