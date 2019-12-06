using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegalLead.Changed.Models
{
    public static class StringExtensions
    {
        public static string ToFixedWidth(this string data, int length)
        {
            if (length <= 0) return string.Empty;
            if (string.IsNullOrEmpty(data))
            {
                return new string(' ', length);
            }
            var sb = new StringBuilder(data);
            while (sb.Length < length)
            {
                sb.Append(" ");
            }
            return sb.ToString().Substring(0, length);
        }

        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }

        public static IEnumerable<string> SplitByLength(this string str, int maxLength, bool removeBlank)
        {
            var result = str.SplitByLength(maxLength).ToList();
            // if the last element is an empty string, remove it
            var lastElement = result.Last().Trim();
            if (!string.IsNullOrEmpty(lastElement)) return result;
            result.RemoveAt(result.Count - 1);
            return result;

        }
    }
}
