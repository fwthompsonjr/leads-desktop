using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Extensions
{
    internal static class ObjectExtensions
    {
        public static List<ValidationResult> Validate<T>(this T source, out bool isValid) where T : class
        {
            var context = new ValidationContext(source, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(source, context, validationResults, true);
            return validationResults;
        }
        public static T ChangeType<T>(this object source, T defaultValue = default)
        {
            var culture = CultureInfo.CurrentCulture;
            if (DBNull.Value == source || source == null) return defaultValue;
            Type t = typeof(T);
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (t == typeof(string))
            {
                var temp = Convert.ToString(source, culture);
                if (temp == null) return defaultValue;
                return (T)Convert.ChangeType(temp, t, culture);
            }
            return (T)Convert.ChangeType(source, t, culture) ?? defaultValue;
        }

        public static List<QueryDbResponse> ConvertFrom(this List<PersonAddress> addresses)
        {
            var list = new List<QueryDbResponse>();
            if (addresses == null) return list;
            addresses.ForEach(a => list.Add(new QueryDbResponse
            {
                Name = a.Name ?? string.Empty,
                Zip = a.Zip ?? string.Empty,
                Address1 = a.Address1 ?? string.Empty,
                Address2 = a.Address2 ?? string.Empty,
                Address3 = a.Address3 ?? string.Empty,
                CaseNumber = a.CaseNumber ?? string.Empty,
                DateFiled = a.DateFiled ?? string.Empty,
                Court = a.Court ?? string.Empty,
                CaseType = a.CaseType ?? string.Empty,
                CaseStyle = a.CaseStyle ?? string.Empty,
                Plaintiff = a.Plantiff ?? string.Empty,
            }));
            return list;

        }
        public static void BindGrid(this List<QueryDbResponse> source, DataGridView view, bool isMasked = false)
        {
            
            // Clear existing columns
            view.Columns.Clear();
            if (isMasked)
            {
                var masked = source.Obfuscate().ToList();
                view.DataSource = masked;
                view.Refresh();
                return;
            }
            // Set the DataSource
            view.DataSource = source;
            view.Refresh();
        }
        /// <summary>
        /// Replaces all non-whitespace characters in the input string with the specified mask character.
        /// </summary>
        /// <param name="input">The input string to be obfuscated.</param>
        /// <param name="maskCharacter">The character to replace non-whitespace characters with. Defaults to '*'.</param>
        /// <returns>A new string where all non-whitespace characters are replaced with the mask character.</returns>
        public static string Obfuscate(this string input, char maskCharacter = '*')
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            char[] result = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = char.IsWhiteSpace(input[i]) ? input[i] : maskCharacter;
            }

            return new string(result);
        }
        public static IEnumerable<QueryDbResponse> Obfuscate(this IEnumerable<QueryDbResponse> data)
        {
            return data == null
                ? throw new ArgumentNullException(nameof(data))
                : data.Select(item => new QueryDbResponse
            {
                DateFiled = item.DateFiled,
                CaseNumber = ObfuscationStrategies.Strategies[QueryDbResponseFieldName.CaseNumber](item),
                Name = ObfuscationStrategies.Strategies[QueryDbResponseFieldName.Name](item),
                Address1 = ObfuscationStrategies.Strategies[QueryDbResponseFieldName.Address1](item),
                Address2 = ObfuscationStrategies.Strategies[QueryDbResponseFieldName.Address2](item),
                Address3 = ObfuscationStrategies.Strategies[QueryDbResponseFieldName.Address3](item),
                Zip = ObfuscationStrategies.Strategies[QueryDbResponseFieldName.Zip](item),
                Court = item.Court,
                CaseType = item.CaseType,
                CaseStyle = "** redacted **",
                Plaintiff = ObfuscationStrategies.Strategies[QueryDbResponseFieldName.Plaintiff](item)
            });
        }
        private static class ObfuscationStrategies
        {
            public static readonly Dictionary<string, Func<QueryDbResponse, string>> Strategies = new()
            {
        { QueryDbResponseFieldName.DateFiled, response => response.DateFiled },
        { QueryDbResponseFieldName.CaseNumber, response =>
            {
                var items = response.CaseNumber.Split('-').ToList();
                for (var i = 0; i < items.Count; i++)
                {
                    if (i == 0) continue;
                    items[i] = items[i].Obfuscate('#');
                }
                return string.Join('-', items);
            }
        },
        { QueryDbResponseFieldName.Name, response => response.Name.Obfuscate() },
        { QueryDbResponseFieldName.Address1, response => response.Address1.Obfuscate() },
        { QueryDbResponseFieldName.Address2, response => response.Address2.Obfuscate() },
        { QueryDbResponseFieldName.Address3, response => response.Address3.Obfuscate() },
        { QueryDbResponseFieldName.Zip, response =>
            {
                if (string.IsNullOrWhiteSpace(response.Zip)) return string.Empty;
                if (response.Zip.Length <= 3) return response.Zip;
                var suffix = response.Zip[3..].Obfuscate();
                return string.Concat(response.Zip[..3], suffix);
            }
        },
        { QueryDbResponseFieldName.Court, response => response.Court },
        { QueryDbResponseFieldName.CaseType, response => response.CaseType },
        { QueryDbResponseFieldName.CaseStyle, response => "** redacted **" },
        { QueryDbResponseFieldName.Plaintiff, response => response.Plaintiff.Obfuscate() }
    };
        }

        internal static class QueryDbResponseFieldName
        {
            public const string DateFiled = "DateFiled";
            public const string CaseNumber = "CaseNumber";
            public const string Name = "Name";
            public const string Address1 = "Address1";
            public const string Address2 = "Address2";
            public const string Address3 = "Address3";
            public const string Zip = "Zip";
            public const string Court = "Court";
            public const string CaseType = "CaseType";
            public const string CaseStyle = "CaseStyle";
            public const string Plaintiff = "Plaintiff";
        }
    }
}
