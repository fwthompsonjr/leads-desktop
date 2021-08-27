using System;
using System.Globalization;

namespace Harris.Criminal.Db
{
    public static class DateTimeExtensions
    {

        public static DateTime ToExactDate(
            this string input,
            string dateFormat,
            DateTime dateDefault)
        {
            if (string.IsNullOrEmpty(input) | string.IsNullOrEmpty(dateFormat))
            {
                return dateDefault;
            }
            var culture = CultureInfo.InvariantCulture;
            var style = DateTimeStyles.AssumeLocal;
            if (DateTime.TryParseExact(input, dateFormat, culture, style, out DateTime dte))
            {
                return dte;
            }
            return dateDefault;
        }
    }
}
