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

        public static string ToExactDateString(
            this string input,
            string dateFormat,
            string dateDefault)
        {
            if (string.IsNullOrEmpty(input) | string.IsNullOrEmpty(dateFormat))
            {
                return dateDefault;
            }
            var culture = CultureInfo.InvariantCulture;
            var style = DateTimeStyles.AssumeLocal;
            if (DateTime.TryParseExact(input, dateFormat, culture, style, out DateTime dte))
            {
                return dte.ToString(dateFormat, culture);
            }
            return dateDefault;
        }

        public static DateTime NextMonday()
        {
            DateTime today = DateTime.Today.Date;
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            //if today is monday, add seven days
            if (daysUntilMonday == 0)
            {
                daysUntilMonday = 7;
            }

            return today.AddDays(daysUntilMonday);
        }
    }
}
